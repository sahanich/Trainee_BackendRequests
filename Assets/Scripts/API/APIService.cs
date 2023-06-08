using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Proyecto26;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace API
{
    public static class APIService
    {
        public const string AuthUrl = "https://dev.sensetower.io/accounts/api/v1/accounts/identity/logon";
        public const string GetMyImagesUrl = "https://dev.sensetower.io/images/api/v1/images/get";
        public const string GetMySpacePageUrl = "https://dev.sensetower.io/spaces/api/v1/spaces/info/owned";
        public const string ReplaceAllMyPlacePicturesUrl = "https://dev.sensetower.io/spaces/api/v1/spaces/images/replaceall";

        public static AuthData AuthData = new();

        public static async UniTask<bool> Auth()
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var url = AuthUrl;

            var form = new WWWForm();
            form.AddField("login", "TestMySpaceUser1");
            form.AddField("password", "TestMySpace###119!");

            var result = await Post<AuthData>(url, form);

            if (ReferenceEquals(result, null))
            {
                utcs.TrySetResult(false);
            }
            else
            {
                AuthData = result;
                utcs.TrySetResult(true);
            }

            return await utcs.Task;
        }

        public static async UniTask<UserImageData[]> GetUserImages()
        {
            var utcs = new UniTaskCompletionSource<UserImageData[]>();

            await CheckAuth();

            var options = new RequestHelper()
            {
                Uri = $"{GetMyImagesUrl}?_={DateTime.Now.Millisecond}",
                Headers = new Dictionary<string, string>()
            {
                {"Authorization", $"Bearer {AuthData.AccessToken}"}
            }
            };

            RestClient.Get(options).Then(response =>
            {
                utcs.TrySetResult(DeserializeData<UserImageData[]>(response));
            })
            .Catch(err =>
            {
                Debug.LogWarning($"{nameof(GetUserImages)}. {err.Message}");
                utcs.TrySetResult(new UserImageData[0]);
            });

            return await utcs.Task;
        }

        public static async UniTask<UserSpaceData[]> GetUserSpaces()
        {
            var utcs = new UniTaskCompletionSource<UserSpaceData[]>();

            await CheckAuth();

            var options = new RequestHelper()
            {
                Uri = $"{GetMySpacePageUrl}?_={DateTime.Now.Millisecond}",
                Headers = new Dictionary<string, string>()
            {
                {"Authorization", "Bearer " + AuthData.AccessToken}
            }
            };

            RestClient.Get(options).Then(response =>
            {
                utcs.TrySetResult(DeserializeData<UserSpaceData[]>(response));
            })
            .Catch(err =>
            {
                Debug.LogWarning($"{nameof(GetUserSpaces)}. {err.Message}");
                utcs.TrySetResult(new UserSpaceData[0]);
            });

            return await utcs.Task;
        }

        public static async UniTask<bool> ReplaceAllMyPlacePictures(Guid myPlaceId,
            Dictionary<int, UserImageData> myPlaceImages)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await CheckAuth();

            var form = new Dictionary<string, string>
        {
            { "SpaceId", myPlaceId.ToString() }
        };

            int counter = 0;
            foreach (var i in myPlaceImages)
            {
                form.Add($"Images[{counter}][location]", i.Key.ToString());
                form.Add($"Images[{counter}][imageId]", i.Value.Id.ToString());
                counter++;
            }

            var options = new RequestHelper()
            {
                Uri = ReplaceAllMyPlacePicturesUrl,
                SimpleForm = form,
                Headers = new Dictionary<string, string>()
            {
                {"Authorization", "Bearer " + AuthData.AccessToken}
            }
            };

            RestClient.Put(options).Then(response =>
            {
                utcs.TrySetResult(true);
            })
            .Catch(err =>
            {
                Debug.LogWarning($"{nameof(ReplaceAllMyPlacePictures)}. {err.Message}");
                utcs.TrySetResult(false);
            });

            return await utcs.Task;
        }

        private static async UniTask<T> Post<T>(string Url, WWWForm data) where T : class
        {
            var utcs = new UniTaskCompletionSource<T>();

            var options = new RequestHelper
            {
                FormData = data,
                Uri = Url
            };

            RestClient.Post(options).Then(response =>
            {
                Debug.LogWarning(response.Text);
                utcs.TrySetResult(DeserializeData<T>(response));
            })
            .Catch(err =>
            {
                Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Post)}. {err.Message}. Url: {options.Uri}");
                utcs.TrySetResult(DeserializeData<T>(null));
            });

            return await utcs.Task;
        }

        private static async UniTask CheckAuth()
        {
            if (string.IsNullOrEmpty(AuthData.AccessToken))
            {
                await Auth();
                if (APIService.AuthData == null)
                {
                    Debug.LogWarning($"{nameof(CheckAuth)}. Auth error.");
                    return;
                }
            }
        }

        private static T DeserializeData<T>(ResponseHelper response) where T : class
        {
            T deserialized = null;
            try
            {
                deserialized = JsonConvert.DeserializeObject<T>(response.Text);
            }
            catch (Exception e)
            {
                Debug.Log($"<color=red>Error deserialize {typeof(T)}: </color>" + e);
            }

            return deserialized;
        }
    }
}
