using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomValidators
{
    public class UploadVideoValidator : ComponentBase
    {
        private ValidationMessageStore messageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(UploadVideoValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(UploadVideoValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            messageStore = new(CurrentEditContext);
            messageStore.Clear();
            CurrentEditContext.OnValidationRequested += (s, e) =>
            {
                messageStore.Clear();
                ValidateVideoSource();
            };
            CurrentEditContext.OnFieldChanged += (s, e) =>
            {
                messageStore.Clear(e.FieldIdentifier);
                ValidateVideoSource();
            };
        }

        private void ValidateVideoSource()
        {
            var model = this.CurrentEditContext.Model as UploadVideoModel;
            if (model.UseSourceUrl && String.IsNullOrWhiteSpace(model.SourceUrl))
            {
                var sourceUrlFieldIdentifier = this.CurrentEditContext.Field(nameof(UploadVideoModel.SourceUrl));
                messageStore.Add(sourceUrlFieldIdentifier, $"Please specify a Source Url");
            }
            else if (!model.UseSourceUrl && String.IsNullOrWhiteSpace(model.StoredFileName))
            {
                var storedFileNameFieldIdentifier = this.CurrentEditContext.Field(nameof(UploadVideoModel.StoredFileName));
                messageStore.Add(storedFileNameFieldIdentifier, $"Please upload a file");
            }
        }

        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var err in errors)
            {
                messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            messageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
