namespace Cat_Paw_Footprint.Areas.Helper
{
    public static class ImgBBHelper
    {
        private static readonly string _apiKey = "3fc954197e2bec2064447104ca6e8d7b";

        // 單張圖片上傳
        public static async Task<string> UploadSingleImageAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            string base64Image = Convert.ToBase64String(fileBytes);

            using var client = new HttpClient();
            var content = new MultipartFormDataContent
            {
                { new StringContent(_apiKey), "key" },
                { new StringContent(base64Image), "image" }
            };

            var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
            var result = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

            return json.data.url;
        }


        // 多張圖片上傳
        public static async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
        {
            var urls = new List<string>();
            using var client = new HttpClient();

            foreach (var file in files)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                string base64Image = Convert.ToBase64String(fileBytes);

                var content = new MultipartFormDataContent
                {
                    { new StringContent(_apiKey), "key" },
                    { new StringContent(base64Image), "image" }
                };

                var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
                var result = await response.Content.ReadAsStringAsync();
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                urls.Add(json.data.url.ToString());
            }
            return urls;
        }
    }
}
