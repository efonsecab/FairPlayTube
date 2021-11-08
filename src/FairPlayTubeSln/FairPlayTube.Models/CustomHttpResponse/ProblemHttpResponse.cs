﻿namespace FairPlayTube.Models.CustomHttpResponse
{
    /// <summary>
    /// Represents an Http Problem Response
    /// </summary>
    public class ProblemHttpResponse
    {
        /// <summary>
        /// Type of Problem
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Problem's Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Status Code
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Problem's Detail
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// Trace Id
        /// </summary>
        public string TraceId { get; set; }
        /// <summary>
        /// Errors
        /// </summary>
        public Errors Errors { get; set; }

    }
}

/// <summary>
/// Errors
/// </summary>
#pragma warning disable CA1050 // Declare types in namespaces
public class Errors
#pragma warning restore CA1050 // Declare types in namespaces
{
    /// <summary>
    /// Error messages
    /// </summary>
    public string[] _ { get; set; }
}
