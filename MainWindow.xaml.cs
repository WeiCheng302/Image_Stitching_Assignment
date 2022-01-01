using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Drawing;
using Microsoft.Win32;
using ImageProcessing;
using LOC;
using LOC.Photogrammetry;
using LOC.FeatureMatching;
//using LOC.Photogrammetry.CoordinateTransform;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;


namespace CRP_Assignments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public static string InputIMGName, InputIMGName2, IMGjustProcessed, Savedir, SaveStitching, SaveStitching__Img01, SaveStitching__Img02, SaveMatches = System.Environment.CurrentDirectory + "/Datasets/Match.png";
        public static bool sim_trans = false, aff_trans = false, proj_trans = false, ShowTransformIMG = false, Stitched = false;
        public static List<string> Image = new List<string>();
        public static LOCImage OriginalImage; //LOCImage ProcessImage = new LOCImage;
        public static LOCImage OriginalImage2;
        public static InOrientationCV IOPSet = new InOrientationCV();
        public int OriImgW, OriImgH, OriImgCount, presscount = 0, stitchingcount = 0;
        public double OriImgDPIX, OriImgDPIY;
        public static int count = 0;
        ProjectiveTransform InvPT, ForPT;


        public MainWindow()
        {
            InitializeComponent();
            funcswitcher(false);
        }

        //Basic Function
        private void funcswitcher(bool OnOff)
        {
            Show_B_Band.IsEnabled = OnOff;
            Show_G_Band.IsEnabled = OnOff;
            Show_R_Band.IsEnabled = OnOff;
            Convert2gray.IsEnabled = OnOff;
            Convert2negative.IsEnabled = OnOff;
            Similarity_Transformation.IsEnabled = OnOff;
            Affine_Tramsformation.IsEnabled = OnOff;
            Projective_Transformation.IsEnabled = OnOff;
            SetParameter.IsEnabled = OnOff;
            Transform.IsEnabled = OnOff;

            presscount = 0;
        }
        private void setIMGinfo(LOCImage IMG)
        {
            OriImgW = IMG.Width;
            OriImgH = IMG.Height;
            OriImgDPIX = IMG.DpiX;
            OriImgDPIY = IMG.DpiY;
            OriImgCount = OriImgH * OriImgW;
        }
        private void generateIMG(LOCImage ProcessImage, int target)
        {
            switch (target)
            {
                case 0:
                case 1:
                case 2:
                    for (int i = 0; i < OriImgW; i++)
                    {
                        for (int j = 0; j < OriImgH; j++)
                        {
                            if ((j * OriImgW + i) % 3 == target)
                            {
                                ProcessImage.ByteData[j * OriImgW + i] = OriginalImage.ByteData[j * OriImgW + i];
                                ProcessImage.ByteData[j * OriImgW + i + OriImgCount] = OriginalImage.ByteData[j * OriImgW + i + OriImgCount];
                                ProcessImage.ByteData[j * OriImgW + i + OriImgCount * 2] = OriginalImage.ByteData[j * OriImgW + i + OriImgCount * 2];
                            }
                        }
                    }
                    break;

                case 3:
                    for (int i = 0; i < OriImgW; i++)
                    {
                        for (int j = 0; j < OriImgH; j++)
                        {
                            ProcessImage.ByteData[j * OriImgW + i] = (byte)(((OriginalImage.ByteData[j * 3 * OriImgW + 3 * i]) + (OriginalImage.ByteData[j * 3 * OriImgW + 3 * i + 1]) + (OriginalImage.ByteData[j * 3 * OriImgW + 3 * i + 2])) / 3);
                        }
                    }
                    break;

                case 4:
                    for (int i = 0; i < OriImgW; i++)
                    {
                        for (int j = 0; j < OriImgH; j++)
                        {
                            ProcessImage.ByteData[j * OriImgW + i] = (byte)(255 - OriginalImage.ByteData[j * OriImgW + i]);
                            ProcessImage.ByteData[j * OriImgW + i + OriImgCount] = (byte)(255 - OriginalImage.ByteData[j * OriImgW + i + OriImgCount]);
                            ProcessImage.ByteData[j * OriImgW + i + OriImgCount * 2] = (byte)(255 - OriginalImage.ByteData[j * OriImgW + i + OriImgCount * 2]);
                        }
                    }
                    break;
            }
        }
        public void TurnOnIMGProcess(CheckBox Checkboxes)
        {
            Show_R_Band.IsChecked = false;
            Show_B_Band.IsChecked = false;
            Show_G_Band.IsChecked = false;
            Convert2gray.IsChecked = false;
            Convert2negative.IsChecked = false;

            Checkboxes.IsChecked = true;
        }
        public void TurnOnIMGTransform(CheckBox Checkboxes, int trans)
        {
            sim_trans = (trans == 0) ? true : false;
            aff_trans = (trans == 1) ? true : false;
            proj_trans = (trans == 2) ? true : false;

            Similarity_Transformation.IsChecked = (Checkboxes == Similarity_Transformation) ? true : false;
            Affine_Tramsformation.IsChecked = (Checkboxes == Affine_Tramsformation) ? true : false;
            Projective_Transformation.IsChecked = (Checkboxes == Projective_Transformation) ? true : false;
        }
        private bool NumberofBandsCheck(LOCImage Image1, LOCImage Image2)
        {
            if (Image1.PixelFormat != Image2.PixelFormat) return false;
            else return true;
        }
        private void SaveAndShow(LOCImage ProcessImage, string filename)
        {
            string Processedfile = Savedir + "//" + filename;
            IMGjustProcessed = Processedfile;
            ProcessImage.Save(Processedfile, ImageFormat.Png);

            using (var stream = new FileStream(Processedfile, FileMode.Open, FileAccess.Read, FileShare.Read)) Img02.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
        private void SetCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (InputIMGName == null) MessageBox.Show("No images have been processed");
            else
            {
                InputIMGName = IMGjustProcessed;
                OriginalImage = new LOCImage(InputIMGName, Int32Rect.Empty);
                using (var stream = new FileStream(InputIMGName, FileMode.Open, FileAccess.Read, FileShare.Read)) Img01.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                setIMGinfo(OriginalImage);
            }
        }
        private void SetParameter_Click(object sender, RoutedEventArgs e)
        {
            Transformation_parameter Enter_Parameter = new Transformation_parameter();

            if (sim_trans == true || aff_trans == true || proj_trans == true) Enter_Parameter.ShowDialog();
            else MessageBox.Show("Select Your Type of Transformation First !!!");
        }
        private void SetAsLeft_Click(object sender, RoutedEventArgs e)
        {
            if (InputIMGName == null) MessageBox.Show("No images have been processed");
            else
            {
                InputIMGName = SaveStitching;
                OriginalImage = new LOCImage(SaveStitching, Int32Rect.Empty);
                using (var stream = new FileStream(SaveStitching, FileMode.Open, FileAccess.Read, FileShare.Read)) Img01.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                setIMGinfo(OriginalImage);
            }
        }
        private void SurfAnalysis_Click(object sender, RoutedEventArgs e)
        {
            var main = new SURF_Analysis();
            main.Show();
        }

        //Load Images
        private void LoadIMG_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.ShowDialog();

            if (OFD.FileName == null) MessageBox.Show("Select an Image!");
            else
            {
                InputIMGName = OFD.FileName;
                Img01.Source = new BitmapImage(new Uri(InputIMGName));
                Savedir = Path.GetDirectoryName(InputIMGName);
                InputIMGLabel.Content = InputIMGName;
                Image.Clear();
                Image.Add(InputIMGName);

                OriginalImage = new LOCImage(InputIMGName, Int32Rect.Empty);
                setIMGinfo(OriginalImage);
                funcswitcher(true);
            }            
        }
        private void LoadIMG2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.ShowDialog();

            if (OFD.FileName == null) MessageBox.Show("Select an Image!");
            else
            {
                InputIMGName2 = OFD.FileName;
                Img02.Source = new BitmapImage(new Uri(InputIMGName2));
                InputIMGLabel.Content = InputIMGName2;
                Image.Add(InputIMGName2);

                OriginalImage2 = new LOCImage(InputIMGName2, Int32Rect.Empty);
                setIMGinfo(OriginalImage2);
                funcswitcher(true);
            }            
        }        
        private void LoadAll_Click(object sender, RoutedEventArgs e)
        {
            //Put all the file in the folder to a list
            InputIMGLISTLabel.Content = "Images in " + Path.GetDirectoryName(InputIMGName) + " have been loaded";
            Savedir = Path.GetDirectoryName(InputIMGName);

            foreach (string filename in Directory.GetFileSystemEntries(Path.GetDirectoryName(InputIMGName),"*.png"))
            {
                if (filename != InputIMGName) Image.Add(filename);
            }
        }
        private void Finland_Dataset_Click(object sender, RoutedEventArgs e)
        {
            InputIMGName = System.Environment.CurrentDirectory + "/Datasets/German01.jpg";
            InputIMGName2 = System.Environment.CurrentDirectory + "/Datasets/German02.jpg";

            Img01.Source = new BitmapImage(new Uri(InputIMGName));
            Img02.Source = new BitmapImage(new Uri(InputIMGName2));
            Savedir = Path.GetDirectoryName(InputIMGName);
            InputIMGLabel.Content = InputIMGName;
            IMGjustProcessed = InputIMGName2;
            SaveMatches = Savedir + "/Finland_Match.png";

            OriginalImage = new LOCImage(InputIMGName, Int32Rect.Empty);
            OriginalImage2 = new LOCImage(InputIMGName2, Int32Rect.Empty);
            setIMGinfo(OriginalImage);
            funcswitcher(true);
        }
        private void Paris_Dataset_Click(object sender, RoutedEventArgs e)
        {
            InputIMGName = System.Environment.CurrentDirectory + "/Datasets/Paris01.jpg";
            InputIMGName2 = System.Environment.CurrentDirectory + "/Datasets/Paris02.jpg";

            Img01.Source = new BitmapImage(new Uri(InputIMGName));
            Img02.Source = new BitmapImage(new Uri(InputIMGName2));
            Savedir = Path.GetDirectoryName(InputIMGName);
            InputIMGLabel.Content = InputIMGName;
            IMGjustProcessed = InputIMGName2;
            SaveMatches = Savedir + "/Paris_Match.png";

            OriginalImage = new LOCImage(InputIMGName, Int32Rect.Empty);
            OriginalImage2 = new LOCImage(InputIMGName2, Int32Rect.Empty);
            setIMGinfo(OriginalImage);
            funcswitcher(true);
        }
        private void Belgium_Dataset_Click(object sender, RoutedEventArgs e)
        {
            InputIMGName = System.Environment.CurrentDirectory + "/Datasets/Belgium01.jpg";
            InputIMGName2 = System.Environment.CurrentDirectory + "/Datasets/Belgium02.jpg";

            Img01.Source = new BitmapImage(new Uri(InputIMGName));
            Img02.Source = new BitmapImage(new Uri(InputIMGName2));
            Savedir = Path.GetDirectoryName(InputIMGName);
            InputIMGLabel.Content = InputIMGName;
            IMGjustProcessed = InputIMGName2;
            SaveMatches = Savedir + "/Belgium_Match.png";

            OriginalImage = new LOCImage(InputIMGName, Int32Rect.Empty);
            OriginalImage2 = new LOCImage(InputIMGName2, Int32Rect.Empty);
            setIMGinfo(OriginalImage);
            funcswitcher(true);
        }
        private void Iceland_Dataset_Click(object sender, RoutedEventArgs e)
        {
            InputIMGName = System.Environment.CurrentDirectory + "/Datasets/Iceland01.jpg";
            InputIMGName2 = System.Environment.CurrentDirectory + "/Datasets/Iceland02.jpg";

            Img01.Source = new BitmapImage(new Uri(InputIMGName));
            Img02.Source = new BitmapImage(new Uri(InputIMGName2));
            Savedir = Path.GetDirectoryName(InputIMGName);
            InputIMGLabel.Content = InputIMGName;
            IMGjustProcessed = InputIMGName2;
            SaveMatches = Savedir + "/Iceland_Match.png";

            OriginalImage = new LOCImage(InputIMGName, Int32Rect.Empty);
            OriginalImage2 = new LOCImage(InputIMGName2, Int32Rect.Empty);
            setIMGinfo(OriginalImage);
            funcswitcher(true);
        }

        //Distortion
        private void Undistorted_Img_Click(object sender, RoutedEventArgs e)
        {
            var main = new Distortion_Parameter();
            main.Show();
        }

        //Image Matching
        private void Image_Matching_Click(object sender, RoutedEventArgs e)
        {
            RMSE_Controller(false, false, null);

            InOrietation IO = new InOrietation(OriginalImage.Width, OriginalImage.Height, 1f, 1f);

            int keys = int.Parse(number_of_features_txt.Text);
            Int32 scale = Int32.Parse(scale_txt.Text);
            bool rotation = bool.Parse(rotation_txt.Text);
            float threshold = float.Parse(Matching_threshold_txt.Text);

            ImageMatching.SURF SURF = new ImageMatching.SURF(keys, keys, (SURF_Scale)scale, rotation, false, threshold);

            SURF.FeatureMatching(OriginalImage, OriginalImage2, Int32Rect.Empty, Int32Rect.Empty);
            SURF.GetMatches(IO, IO, true);

            InvPT = new ProjectiveTransform(2.5f);
            ForPT = new ProjectiveTransform(2.5f);

            LeastSquare LSA_forward = new LeastSquare();
            LeastSquare LSA_Inverse = new LeastSquare();

            ForPT.Adjustment(SURF.TarMatchPTs, SURF.RefMatchPTs, LSA_forward);
            InvPT.Adjustment(SURF.RefMatchPTs, SURF.TarMatchPTs, LSA_Inverse);

            SURF.SaveMatches(System.Environment.CurrentDirectory + "/Matches.txt");
            SURF.PaintSURF(SaveMatches, new Bitmap(InputIMGName), new Bitmap(InputIMGName2), System.Drawing.Color.Green, 5, 1);

            using (var stream = new FileStream(SaveMatches, FileMode.Open, FileAccess.Read, FileShare.Read)) Img03.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            RMSE_Controller(true, true, "RMSE =" + Convert.ToString(LSA_Inverse.RMSE));
            Stitched = false;
        }
        private void StitchingAndSave_Click(object sender, RoutedEventArgs e)
        {
            if (Stitched == false)
            {
                List<ImagePoints.Coordinate> Corner = new List<ImagePoints.Coordinate>
                {
                new ImagePoints.Coordinate(0, 0, CoordinateFormat.Pixel),
                new ImagePoints.Coordinate(OriginalImage.Width, 0, CoordinateFormat.Pixel),
                new ImagePoints.Coordinate(OriginalImage.Width, OriginalImage.Height, CoordinateFormat.Pixel),
                new ImagePoints.Coordinate(0, OriginalImage.Height, CoordinateFormat.Pixel)
                };

                InOrietation IO = new InOrietation(OriginalImage.Width, OriginalImage.Height, 1f, 1f);
                for (int i = 0; i < 4; i++) Corner[i].FormatTransform(IO, CoordinateFormat.mm);

                Corner = InvPT.Transform(Corner);

                Corner.Add(new ImagePoints.Coordinate(0, 0, CoordinateFormat.Pixel));
                Corner.Add(new ImagePoints.Coordinate(OriginalImage.Width, 0, CoordinateFormat.Pixel));
                Corner.Add(new ImagePoints.Coordinate(OriginalImage.Width, OriginalImage.Height, CoordinateFormat.Pixel));
                Corner.Add(new ImagePoints.Coordinate(0, OriginalImage.Height, CoordinateFormat.Pixel));

                for (int i = 4; i < 8; i++) Corner[i].FormatTransform(IO, CoordinateFormat.mm);

                float Max_x = -999999f, Max_y = -9999999f, Min_x = 999999f, Min_y = 999999f;

                for (int i = 0; i < 8; i++)
                {
                    if (Corner[i].X > Max_x) Max_x = Corner[i].X;
                    if (Corner[i].Y > Max_y) Max_y = Corner[i].Y;
                    if (Corner[i].X < Min_x) Min_x = Corner[i].X;
                    if (Corner[i].Y < Min_y) Min_y = Corner[i].Y;
                }

                int StitchWidth = (int)(Max_x - Min_x);
                int StitchHeight = (int)(Max_y - Min_y);
                int NewStitchWidth = -1; int NewStitchHeight = -1;
                int OffsetWidth = (int)(Max_x + Min_x) / -2;
                int OffsetHeight = (int)(Max_y + Min_y) / -2;

                byte[] Imagebyte = new byte[StitchWidth * StitchHeight * OriginalImage.NumberOfBands];
                int Width = OriginalImage.Width; int Height = OriginalImage.Height; int Bands = OriginalImage.NumberOfBands;

                for (int i = 0; i < StitchWidth; i++)
                {
                    int Index = 0;
                    float refx = 0, refy = 0, tarx = 0, tary = 0, stitchx = 0, stitchy = 0;

                    for (int j = 0; j < StitchHeight; j++)
                    {
                        Index = (j * StitchWidth + i) * Bands;

                        stitchx = (i - StitchWidth / 2) + OffsetWidth;
                        stitchy = (StitchHeight / 2 - j) + OffsetHeight;

                        refx = stitchx + Width / 2;
                        refy = -stitchy + Height / 2;

                        for (int k = 0; k < Bands; k++)
                        {
                            Imagebyte[Index + k] = (byte)Interpolation.Bilinear(OriginalImage, refx, refy, k);
                        }

                        InvPT.Transform(stitchx, stitchy);

                        tarx = (InvPT.TransformPt[0] + Width / 2);
                        tary = (-InvPT.TransformPt[1] + Height / 2);

                        for (int k = 0; k < Bands; k++)
                        {
                            if (Imagebyte[Index + k] == 0) Imagebyte[Index + k] = (byte)Interpolation.Bilinear(OriginalImage2, tarx, tary, k);
                        }
                    }
                }

                for (int i = Imagebyte.Length - 1; i > 0; i--)
                {
                    double h = i / (Bands * StitchWidth); int w = i % (Bands * StitchWidth);
                    if (Imagebyte[i] != 0 && Convert.ToInt32(h) > NewStitchHeight) NewStitchHeight = Convert.ToInt32(Math.Round(h));
                    if (Imagebyte[i] != 0 && i % w > NewStitchWidth) NewStitchWidth = Convert.ToInt32((i % w));
                }

                NewStitchWidth = NewStitchWidth / Bands;

                byte[] Imagebyte_Clip = new byte[NewStitchWidth * NewStitchHeight * OriginalImage.NumberOfBands];

                for (int i = 0; i < NewStitchWidth; i++)
                {
                    for (int j = 0; j < NewStitchHeight; j++)
                    {
                        int Index = (j * StitchWidth + i) * Bands;
                        int Index2 = (j * NewStitchWidth + i) * Bands;
                        for (int k = 0; k < Bands; k++) Imagebyte_Clip[Index2 + k] = Imagebyte[Index + k];
                    }
                }

                LOCImage StitchIMG = new LOCImage(NewStitchWidth, NewStitchHeight, 96, 96, OriginalImage.PixelFormat, null);

                SaveStitching = System.Environment.CurrentDirectory + "/Datasets/Stitch" + Convert.ToString(stitchingcount) + ".png";
                StitchIMG.ByteData = Imagebyte_Clip;
                StitchIMG.Save(SaveStitching, ImageFormat.Png);
                stitchingcount += 1;

                Stitched = true;
            }

            using (var stream = new FileStream(SaveStitching, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Img03.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            MessageBox.Show("Stitched Image Saved !");
        }      
        private void Registing_Click(object sender, RoutedEventArgs e)
        {
            if (Stitched == false)
            {
                List<ImagePoints.Coordinate> Corner = new List<ImagePoints.Coordinate>
                {
                new ImagePoints.Coordinate(0, 0, CoordinateFormat.Pixel),
                new ImagePoints.Coordinate(OriginalImage.Width, 0, CoordinateFormat.Pixel),
                new ImagePoints.Coordinate(OriginalImage.Width, OriginalImage.Height, CoordinateFormat.Pixel),
                new ImagePoints.Coordinate(0, OriginalImage.Height, CoordinateFormat.Pixel)
                };

                InOrietation IO = new InOrietation(OriginalImage.Width, OriginalImage.Height, 1f, 1f);
                for (int i = 0; i < 4; i++) Corner[i].FormatTransform(IO, CoordinateFormat.mm);

                Corner = InvPT.Transform(Corner);

                Corner.Add(new ImagePoints.Coordinate(0, 0, CoordinateFormat.Pixel));
                Corner.Add(new ImagePoints.Coordinate(OriginalImage.Width, 0, CoordinateFormat.Pixel));
                Corner.Add(new ImagePoints.Coordinate(OriginalImage.Width, OriginalImage.Height, CoordinateFormat.Pixel));
                Corner.Add(new ImagePoints.Coordinate(0, OriginalImage.Height, CoordinateFormat.Pixel));

                for (int i = 4; i < 8; i++) Corner[i].FormatTransform(IO, CoordinateFormat.mm);

                float Max_x = -999999f, Max_y = -9999999f, Min_x = 999999f, Min_y = 999999f;

                for (int i = 0; i < 8; i++)
                {
                    if (Corner[i].X > Max_x) Max_x = Corner[i].X;
                    if (Corner[i].Y > Max_y) Max_y = Corner[i].Y;
                    if (Corner[i].X < Min_x) Min_x = Corner[i].X;
                    if (Corner[i].Y < Min_y) Min_y = Corner[i].Y;
                }

                int StitchWidth = (int)(Max_x - Min_x);
                int StitchHeight = (int)(Max_y - Min_y);
                int NewStitchWidth = -1; 
                int NewStitchHeight = -1;
                int OffsetWidth = (int)(Max_x + Min_x) / -2;
                int OffsetHeight = (int)(Max_y + Min_y) / -2;

                //Stitch Img
                byte[] Imagebyte = new byte[StitchWidth * StitchHeight * OriginalImage.NumberOfBands];
                int Width = OriginalImage.Width; int Height = OriginalImage.Height; int Bands = OriginalImage.NumberOfBands;

                for (int i = 0; i < StitchWidth; i++)
                {
                    int Index = 0;
                    float refx = 0, refy = 0, tarx = 0, tary = 0, stitchx = 0, stitchy = 0;

                    for (int j = 0; j < StitchHeight; j++)
                    {
                        Index = (j * StitchWidth + i) * Bands;

                        stitchx = (i - StitchWidth / 2) + OffsetWidth;
                        stitchy = (StitchHeight / 2 - j) + OffsetHeight;

                        refx = stitchx + Width / 2;
                        refy = -stitchy + Height / 2;

                        for (int k = 0; k < Bands; k++)
                        {
                            Imagebyte[Index + k] = (byte)Interpolation.Bilinear(OriginalImage, refx, refy, k);
                        }

                        InvPT.Transform(stitchx, stitchy);

                        tarx = (InvPT.TransformPt[0] + Width / 2);
                        tary = (-InvPT.TransformPt[1] + Height / 2);

                        for (int k = 0; k < Bands; k++)
                        {
                            if (Imagebyte[Index + k] == 0) Imagebyte[Index + k] = (byte)Interpolation.Bilinear(OriginalImage2, tarx, tary, k);
                        }
                    }
                }

                for (int i = Imagebyte.Length-1; i > 0; i--)
                {
                    double h = i / (Bands * StitchWidth);
                    double w = i % (Bands * StitchWidth);
                    if (Imagebyte[i] != 0 && Convert.ToInt32(h) > NewStitchHeight) NewStitchHeight = Convert.ToInt32(Math.Round(h));
                    if (Imagebyte[i] != 0 && i % w > NewStitchWidth) NewStitchWidth = Convert.ToInt32((i % w));
                }

                NewStitchWidth = NewStitchWidth / Bands;

                byte[] Imagebyte_Clip = new byte[NewStitchWidth * NewStitchHeight * OriginalImage.NumberOfBands];

                for (int i = 0; i < NewStitchWidth; i++)
                {
                    for (int j = 0; j < NewStitchHeight; j++)
                    {
                        int Index = (j * StitchWidth + i) * Bands;
                        int Index2 = (j * NewStitchWidth + i) * Bands;
                        for (int k = 0; k < Bands; k++) Imagebyte_Clip[Index2 + k] = Imagebyte[Index + k];                        
                    }
                }

                LOCImage StitchIMG = new LOCImage(NewStitchWidth, NewStitchHeight, 96, 96, OriginalImage.PixelFormat, null);

                SaveStitching = System.Environment.CurrentDirectory + "/Datasets/Stitch" + Convert.ToString(stitchingcount) + ".png";
                StitchIMG.ByteData = Imagebyte_Clip;
                StitchIMG.Save(SaveStitching, ImageFormat.Png);
                stitchingcount += 1;

                //Left Img 
                byte[] Imagebyte_Img01 = new byte[StitchWidth * StitchHeight * OriginalImage.NumberOfBands];
                
                for (int i = 0; i < StitchWidth; i++)
                {
                    int Index = 0;
                    float refx = 0, refy = 0, tarx = 0, tary = 0, stitchx = 0, stitchy = 0;

                    for (int j = 0; j < StitchHeight; j++)
                    {
                        Index = (j * StitchWidth + i) * Bands;

                        stitchx = (i - StitchWidth / 2) + OffsetWidth;
                        stitchy = (StitchHeight / 2 - j) + OffsetHeight;

                        refx = stitchx + Width / 2;
                        refy = -stitchy + Height / 2;

                        for (int k = 0; k < Bands; k++)
                        {
                            Imagebyte_Img01[Index + k] = (byte)Interpolation.Bilinear(OriginalImage, refx, refy, k);
                        }
                    }
                }

                int NewStitchWidthL = -1;
                int NewStitchHeightL = -1;

                for (int i = Imagebyte.Length - 1; i > 0; i--)
                {
                    double h = i / (Bands * StitchWidth);
                    double w = i % (Bands * StitchWidth);
                    if (Imagebyte[i] != 0 && Convert.ToInt32(h) > NewStitchHeightL) NewStitchHeightL = Convert.ToInt32(Math.Round(h));
                    if (Imagebyte[i] != 0 && i % w > NewStitchWidthL) NewStitchWidthL = Convert.ToInt32((i % w));
                }

                NewStitchWidthL = NewStitchWidthL / Bands;

                byte[] Imagebyte_Clip2 = new byte[NewStitchWidthL * NewStitchHeightL * OriginalImage.NumberOfBands];

                for (int i = 0; i < NewStitchWidthL; i++)
                {
                    for (int j = 0; j < NewStitchHeightL; j++)
                    {
                        int Index = (j * StitchWidth + i) * Bands;
                        int Index2 = (j * NewStitchWidthL + i) * Bands;
                        for (int k = 0; k < Bands; k++) Imagebyte_Clip2[Index2 + k] = Imagebyte_Img01[Index + k];
                    }
                }

                LOCImage StitchIMG2 = new LOCImage(NewStitchWidthL, NewStitchHeightL, 96, 96, OriginalImage.PixelFormat, null);
                SaveStitching__Img01 = System.Environment.CurrentDirectory + "/Datasets/Stitch_Img01.png";
                StitchIMG2.ByteData = Imagebyte_Clip2;
                StitchIMG2.Save(SaveStitching__Img01, ImageFormat.Png);

                //Right Img
                /*
                byte[] Imagebyte_Img02 = new byte[StitchWidth * StitchHeight * OriginalImage.NumberOfBands];

                for (int i = 0; i < StitchWidth; i++)
                {
                    int Index = 0;
                    float refx = 0, refy = 0, tarx = 0, tary = 0, stitchx = 0, stitchy = 0;

                    for (int j = 0; j < StitchHeight; j++)
                    {
                        Index = (j * StitchWidth + i) * Bands;

                        stitchx = (i - StitchWidth / 2) + OffsetWidth;
                        stitchy = (StitchHeight / 2 - j) + OffsetHeight;

                        InvPT.Transform(stitchx, stitchy);

                        tarx = (InvPT.TransformPt[0] + Width / 2);
                        tary = (-InvPT.TransformPt[1] + Height / 2);

                        for (int k = 0; k < Bands; k++)
                        {
                            if (Imagebyte_Img02[Index + k] == 0) Imagebyte_Img02[Index + k] = (byte)Interpolation.Bilinear(OriginalImage2, tarx, tary, k);
                        }
                    }
                }

                int NewStitchWidthR = -1;
                int NewStitchHeightR = -1;

                for (int i = Imagebyte.Length - 1; i > 0; i--)
                {
                    double h = i / (Bands * StitchWidth);
                    double w = i % (Bands * StitchWidth);
                    if (Imagebyte[i] != 0 && Convert.ToInt32(h) > NewStitchHeightR) NewStitchHeightR = Convert.ToInt32(Math.Round(h));
                    if (Imagebyte[i] != 0 && i % w > NewStitchWidthR) NewStitchWidthR = Convert.ToInt32((i % w));
                }

                NewStitchWidthR = NewStitchWidthR / Bands;

                byte[] Imagebyte_Clip3 = new byte[NewStitchWidthR * NewStitchHeightR * OriginalImage.NumberOfBands];

                for (int i = 0; i < NewStitchWidthR; i++)
                {
                    for (int j = 0; j < NewStitchHeightR; j++)
                    {
                        int Index = (j * StitchWidth + i) * Bands;
                        int Index2 = (j * NewStitchWidthR + i) * Bands;
                        for (int k = 0; k < Bands; k++) Imagebyte_Clip3[Index2 + k] = Imagebyte_Img02[Index + k];
                    }
                }

                LOCImage StitchIMG3 = new LOCImage(NewStitchWidthR, NewStitchHeightR, 96, 96, OriginalImage.PixelFormat, null);
                SaveStitching__Img02 = System.Environment.CurrentDirectory + "/Datasets/Stitch_Img02.png";
                StitchIMG3.ByteData = Imagebyte_Img02;
                StitchIMG3.Save(SaveStitching__Img02, ImageFormat.Png);*/

                Stitched = true;
            }

            if (presscount % 2 == 0)
            {
                using (var stream = new FileStream(SaveStitching, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Img03.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
            else if (presscount % 2 != 0)
            {
                using (var stream = new FileStream(SaveStitching__Img01, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Img03.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }

            presscount += 1;
        }
        private void RMSE_Controller(bool Enable, bool Visible, String rmsevalue)
        { 
            RMSE.Content = rmsevalue;
            RMSE.IsEnabled = (Enable == true) ? true : false;
            RMSE.Visibility = (Visible == true) ? Visibility.Visible : Visibility.Hidden;
        }

        //Image Transformation
        private void Transform_Click(object sender, RoutedEventArgs e)
        {
            if (ShowTransformIMG != true) Console.WriteLine("Load an Image and Set the Transformation Parameter First ! ");
            else if (Transformation_parameter.PutLeft == true)
            {
                string Processedfile = Savedir + "//" + Path.GetFileName(IMGjustProcessed);
                IMGjustProcessed = Processedfile;
                InputIMGName = IMGjustProcessed;
                Transformation_parameter.ProcessImage.Save(Processedfile, ImageFormat.Png);

                using (var stream = new FileStream(Processedfile, FileMode.Open, FileAccess.Read, FileShare.Read)) Img01.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                ShowTransformIMG = false;
                OriginalImage = new LOCImage(IMGjustProcessed, Int32Rect.Empty);
            }
            else
            {
                SaveAndShow(Transformation_parameter.ProcessImage, Path.GetFileName(IMGjustProcessed));
                string Processedfile = Savedir + "//" + Path.GetFileName(IMGjustProcessed);
                IMGjustProcessed = Processedfile;
                ShowTransformIMG = false;
                OriginalImage2 = new LOCImage(IMGjustProcessed, Int32Rect.Empty);
                InputIMGName2 = IMGjustProcessed;
            }
        }
        private void ShowRBand_Click(object sender, RoutedEventArgs e)
        {
            TurnOnIMGProcess(Show_R_Band);

            LOCImage ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
            if (!NumberofBandsCheck(OriginalImage, ProcessImage)) MessageBox.Show("Number of Bands don't match.");

            generateIMG(ProcessImage, 2);
            SaveAndShow(ProcessImage, "Hw1_R_Band.png");
        }
        private void ShowGBand_Click(object sender, RoutedEventArgs e)
        {
            TurnOnIMGProcess(Show_G_Band);

            LOCImage ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
            if (!NumberofBandsCheck(OriginalImage, ProcessImage)) MessageBox.Show("Number of Bands don't match.");

            OriginalImage2 = new LOCImage(IMGjustProcessed, Int32Rect.Empty);
            generateIMG(ProcessImage, 1);
            SaveAndShow(ProcessImage, "Hw1_G_Band.png");
        }
        private void ShowBBand_Click(object sender, RoutedEventArgs e)
        {
            TurnOnIMGProcess(Show_B_Band);

            LOCImage ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
            if (!NumberofBandsCheck(OriginalImage, ProcessImage)) MessageBox.Show("Number of Bands don't match.");

            OriginalImage2 = new LOCImage(IMGjustProcessed, Int32Rect.Empty);
            generateIMG(ProcessImage, 0);
            SaveAndShow(ProcessImage, "Hw1_B_Band.png");
        }
        private void Convert2gray_Click(object sender, RoutedEventArgs e)
        {
            TurnOnIMGProcess(Convert2gray);

            LOCImage ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Gray8, null);

            OriginalImage2 = new LOCImage(IMGjustProcessed, Int32Rect.Empty);
            generateIMG(ProcessImage, 3);
            SaveAndShow(ProcessImage, "Hw1_Gray.png");
        }
        private void Convert2negative_Click(object sender, RoutedEventArgs e)
        {
            TurnOnIMGProcess(Convert2negative);

            LOCImage ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
            if (!NumberofBandsCheck(OriginalImage, ProcessImage)) MessageBox.Show("Number of Bands don't match.");

            OriginalImage2 = new LOCImage(IMGjustProcessed, Int32Rect.Empty);
            generateIMG(ProcessImage, 4);
            SaveAndShow(ProcessImage, "Hw1_Negative.png");
        }
        private void Similarity_Transformation_Checked(object sender, RoutedEventArgs e)
        {
            TurnOnIMGTransform(Similarity_Transformation, 0);
        }
        private void Affine_Transformation_Checked(object sender, RoutedEventArgs e)
        {
            TurnOnIMGTransform(Affine_Tramsformation, 1);
        }
        private void Projective_Transformation_Checked(object sender, RoutedEventArgs e)
        {
            TurnOnIMGTransform(Projective_Transformation, 2);
        }   
    }
}
