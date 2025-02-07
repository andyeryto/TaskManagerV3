using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace TaskManager.Application.Services
{
    public class TaskAIService
    {
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _sentimentEngine;

        public TaskAIService()
        {
            _mlContext = new MLContext();
            _sentimentEngine = LoadSentimentModel();
        }

        /// <summary>
        /// AI-based task prioritization using keyword analysis.
        /// </summary>
        public string AnalyzeTaskPriority(string description)
        {
            if (description.Contains("urgent", StringComparison.OrdinalIgnoreCase) ||
                description.Contains("immediately", StringComparison.OrdinalIgnoreCase))
            {
                return "High Priority";
            }
            else if (description.Contains("later", StringComparison.OrdinalIgnoreCase) ||
                     description.Contains("optional", StringComparison.OrdinalIgnoreCase))
            {
                return "Low Priority";
            }
            return "Medium Priority";
        }

        /// <summary>
        /// AI-based sentiment analysis on task descriptions.
        /// </summary>
        public string AnalyzeSentiment(string description)
        {
            var prediction = _sentimentEngine.Predict(new SentimentData { Text = description });

            return prediction.PredictedLabel ? "Positive" : "Negative";
        }

        /// <summary>
        /// Loads the ML.NET sentiment analysis model.
        /// </summary>
        private PredictionEngine<SentimentData, SentimentPrediction> LoadSentimentModel()
        {
            var data = new List<SentimentData>
            {
                new SentimentData { Text = "urgent", Sentiment = true },
                new SentimentData { Text = "important", Sentiment = true },
                new SentimentData { Text = "delayed", Sentiment = false },
                new SentimentData { Text = "later", Sentiment = false }
            };

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimentData.Sentiment)));

            var model = pipeline.Fit(dataView);
            return _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }
    }

    public class SentimentData
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        [ColumnName("Sentiment")]
        public bool Sentiment { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
    }
}
