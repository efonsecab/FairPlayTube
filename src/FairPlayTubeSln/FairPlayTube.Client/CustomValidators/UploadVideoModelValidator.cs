using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomValidators
{
    public class UploadVideoModelValidator : ComponentBase
    {
        private const string MissingFileInput = "FileName and Url are both empty. Please select a file or use the Url field";

        private ValidationMessageStore ValidationMessageStore { get; set; }
        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(UploadVideoModelValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(UploadVideoModelValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            ValidationMessageStore = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
            {
                ValidationMessageStore.Clear();
                var model = this.CurrentEditContext.Model as UploadVideoModel;
                var fileName = model.FileName;
                var fileUrl = model.SourceUrl;
                if (String.IsNullOrWhiteSpace(fileName) && String.IsNullOrWhiteSpace(fileUrl))
                {
                    ValidationMessageStore.Add(this.CurrentEditContext.Field(nameof(UploadVideoModel.FileName)),
                        MissingFileInput);
                }
            };
            CurrentEditContext.OnFieldChanged += (s, e) =>
            {
                ValidationMessageStore.Clear();
                var model = e.FieldIdentifier.Model as UploadVideoModel;
                var fileName = model.FileName;
                var fileUrl = model.SourceUrl;
                if (String.IsNullOrWhiteSpace(fileName) && String.IsNullOrWhiteSpace(fileUrl))
                {
                    ValidationMessageStore.Add(this.CurrentEditContext.Field(nameof(UploadVideoModel.FileName)),
                        MissingFileInput);
                }
            };
        }

        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var err in errors)
            {
                ValidationMessageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            ValidationMessageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
