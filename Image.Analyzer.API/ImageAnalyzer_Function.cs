using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;


namespace Image.Analyzer.API
{
    public static class ImageAnalyzer_Function
    {

        static string subscriptionKey = "COMPUTER_VISION_SUBSCRIPTION_KEY";
        static string endpointKey = "COMPUTER_VISION_ENDPOINT";

        /*
         * AUTHENTICATE
         * Creates a Computer Vision client used by each example.
         */
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        [FunctionName("ImageAnalyzer")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "machine_learning" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "imageUrl", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **imageUrl** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ImageAnalysis), Description = "The OK response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string imageUrl = req.Query["imageUrl"];

            log.LogInformation($"Analyzing image: {imageUrl}");

            var key = Environment.GetEnvironmentVariable(subscriptionKey);
            var endpoint = Environment.GetEnvironmentVariable(endpointKey);

            log.Log(LogLevel.Debug, $"COMPUTER_VISION_SUBSCRIPTION_KEY: {key}");
            log.Log(LogLevel.Debug, $"COMPUTER_VISION_ENDPOINT: {endpoint}");

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new InvalidDataException("The supplied url for the image is not a valid url.");
            }

            ImageAnalysis result = null;
            // Create a client
            ComputerVisionClient client = Authenticate(endpoint, key);
            try
            {
                // Analyze an image to get features and other properties.
                result = await AnalyzeImageUrl(client, imageUrl);

            }
            catch (Exception ex)
            {
                log.LogInformation($"Something went wrong analyzing image: {imageUrl}. \n{ex.StackTrace}");
            }

            return new OkObjectResult(result);
        }

        /* 
         * ANALYZE IMAGE - URL IMAGE
         * Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult/gory content,
         * brands, celebrities, landmarks, color scheme, and image types.
         */
        public static async Task<ImageAnalysis> AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            return await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);
        }
    }
}

