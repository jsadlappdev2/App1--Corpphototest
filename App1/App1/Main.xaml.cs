using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main : ContentPage
    {
        ImageSource _imageSource;
        private IMedia _mediaPicker;
    
        public Main()
        {
            InitializeComponent();
        }

        private async void takephoto_clicked(object sender, EventArgs e)
        {

           await TakePicture();
        }

        private async void pickupphoto_clicked(object sender, EventArgs e)
        {

            await SelectPicture();
        }



        private void Refresh()
        {
            try
            {
                if (App.CroppedImage != null)
                {
                    Stream stream = new MemoryStream(App.CroppedImage);
                    corpimage.Source = ImageSource.FromStream(() => stream);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #region Photos

        private async void Setup()
        {
            if (_mediaPicker != null)
            {
                return;
            }

            ////RM: hack for working on windows phone? 
            await CrossMedia.Current.Initialize();
            _mediaPicker = CrossMedia.Current;
        }

        private async Task SelectPicture()
        {
            Setup();

            _imageSource = null;

            try
            {

                var mediaFile = await this._mediaPicker.PickPhotoAsync();

                _imageSource = ImageSource.FromStream(mediaFile.GetStream);

                var memoryStream = new MemoryStream();
                await mediaFile.GetStream().CopyToAsync(memoryStream);
                byte[] imageAsByte = memoryStream.ToArray();

                await Navigation.PushModalAsync(new CropView(imageAsByte, Refresh));

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task TakePicture()
        {
            Setup();

            _imageSource = null;

            try
            {
                var mediaFile = await this._mediaPicker.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    DefaultCamera = CameraDevice.Front
                });

                _imageSource = ImageSource.FromStream(mediaFile.GetStream);

                var memoryStream = new MemoryStream();
                await mediaFile.GetStream().CopyToAsync(memoryStream);
                byte[] imageAsByte = memoryStream.ToArray();

                await Navigation.PushModalAsync(new CropView(imageAsByte, Refresh));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}


