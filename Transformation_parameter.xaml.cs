using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageProcessing;
using LOC;
using LOC.Photogrammetry;

namespace CRP_Assignments
{
    /// <summary>
    /// Transformation_parameter.xaml 的互動邏輯
    /// </summary>
    public partial class Transformation_parameter : Window
    {
        public static LOCImage ProcessImage, ImagefromMainwindow;
        public static bool PutLeft = false;
        public int OriImgW = MainWindow.OriginalImage.Width;
        public int OriImgH = MainWindow.OriginalImage.Height;
        public double OriImgDPIX = MainWindow.OriginalImage.DpiX;
        public double OriImgDPIY = MainWindow.OriginalImage.DpiY;


        public Transformation_parameter()
        {
            InitializeComponent();
            UnableAll();
            if (MainWindow.sim_trans == true)
            {
                Sim_Tx.IsEnabled = true;
                Sim_Ty.IsEnabled = true;
                Sim_Rot.IsEnabled = true;
                Sim_Scale.IsEnabled = true;
                Sim_Tx_Label.Opacity = 1;
                Sim_Ty_Label.Opacity = 1;
                Sim_Rot_Label.Opacity = 1;
                Sim_Scale_Label.Opacity = 1;
            }
            else if (MainWindow.aff_trans == true)
            {
                Aff_Tx.IsEnabled = true;
                Aff_Ty.IsEnabled = true;
                Aff_Scalex.IsEnabled = true;
                Aff_Scaley.IsEnabled = true;
                Aff_Rot.IsEnabled = true;
                Aff_Shear.IsEnabled = true;
                Aff_Tx_Label.Opacity = 1;
                Aff_Ty_Label.Opacity = 1;
                Aff_Scalex_Label.Opacity = 1;
                Aff_Scaley_Label.Opacity = 1;
                Aff_Rot_Label.Opacity = 1;
                Aff_Shear_Label.Opacity = 1;
            }
            else if (MainWindow.proj_trans == true)
            {
                Proj_Tx.IsEnabled = true;
                Proj_Ty.IsEnabled = true;
                Proj_Tz.IsEnabled = true;
                Proj_Omega.IsEnabled = true;
                Proj_Phi.IsEnabled = true;
                Proj_Kappa.IsEnabled = true;
                Proj_Scalex.IsEnabled = true;
                Proj_Scaley.IsEnabled = true;
                Proj_Tx_Label.Opacity = 1;
                Proj_Ty_Label.Opacity = 1;
                Proj_Tz_Label.Opacity = 1;
                Proj_Omega_Label.Opacity = 1;
                Proj_Phi_Label.Opacity = 1;
                Proj_Kappa_Label.Opacity = 1;
                Proj_Scalex_Label.Opacity = 1;
                Proj_Scaley_Label.Opacity = 1;
            }
        }
        private void UnableAll()
        {
            Sim_Tx.IsEnabled = false;
            Sim_Ty.IsEnabled = false;
            Sim_Rot.IsEnabled = false;
            Sim_Scale.IsEnabled = false;
            Sim_Tx_Label.Opacity = 0.5;
            Sim_Ty_Label.Opacity = 0.5;
            Sim_Rot_Label.Opacity = 0.5;
            Sim_Scale_Label.Opacity = 0.5;

            Aff_Tx.IsEnabled = false;
            Aff_Ty.IsEnabled = false;
            Aff_Scalex.IsEnabled = false;
            Aff_Scaley.IsEnabled = false;
            Aff_Rot.IsEnabled = false;
            Aff_Shear.IsEnabled = false;
            Aff_Tx_Label.Opacity = 0.5;
            Aff_Ty_Label.Opacity = 0.5;
            Aff_Scalex_Label.Opacity = 0.5;
            Aff_Scaley_Label.Opacity = 0.5;
            Aff_Rot_Label.Opacity = 0.5;
            Aff_Shear_Label.Opacity = 0.5;

            Proj_Tx.IsEnabled = false;
            Proj_Ty.IsEnabled = false;
            Proj_Tz.IsEnabled = false;
            Proj_Omega.IsEnabled = false;
            Proj_Phi.IsEnabled = false;
            Proj_Kappa.IsEnabled = false;
            Proj_Scalex.IsEnabled = false;
            Proj_Scaley.IsEnabled = false;
            Proj_Tx_Label.Opacity = 0.5;
            Proj_Ty_Label.Opacity = 0.5;
            Proj_Tz_Label.Opacity = 0.5;
            Proj_Omega_Label.Opacity = 0.5;
            Proj_Phi_Label.Opacity = 0.5;
            Proj_Kappa_Label.Opacity = 0.5;
            Proj_Scalex_Label.Opacity = 0.5;
            Proj_Scaley_Label.Opacity = 0.5;

            TransImg2.IsChecked = false;
            ImagefromMainwindow = MainWindow.OriginalImage;
            PutResultat01.IsChecked = false;
            PutLeft = false;
            MainWindow.count += 1;
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (TransImg2.IsChecked == true) ImagefromMainwindow = MainWindow.OriginalImage2;
            
            if (MainWindow.sim_trans == true)
            {
                float tx = Convert.ToSingle(Sim_Tx.Text);
                float ty = Convert.ToSingle(Sim_Tx.Text);
                float rot = (Convert.ToSingle(Sim_Rot.Text) * (float)(Math.PI)) / 180;
                float scale = Convert.ToSingle(Sim_Scale.Text);
                float tarx = 0, tary = 0;

                ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
                SimilarityTransform Sim = new SimilarityTransform(3);

                Sim.Coeffs[0] = (float)(Math.Cos(rot)) * scale; //(a1)
                Sim.Coeffs[1] = (float)(Math.Sin(rot)) * scale; //(b1)
                Sim.Coeffs[2] = tx; //(a0)
                Sim.Coeffs[3] = ty; //(b0)

                for (int i = 0; i < OriImgW; i++)
                {
                    for (int j = 0; j < OriImgH; j++)
                    {
                        Sim.Transform(i - OriImgW / 2, -j + OriImgH / 2, ref tarx, ref tary);
                        if ((tarx + OriImgW / 2) > 0 && (-tary + OriImgH / 2) > 0 && (tarx + OriImgW / 2) < (OriImgW - 1) && (-tary + OriImgH / 2) < (OriImgH - 1))
                        {
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 1] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 1];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 2] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 2];
                        }
                    }
                }

                MainWindow.IMGjustProcessed = MainWindow.Savedir + "/Hw2_Front_Similarity" + Convert.ToString(MainWindow.count) + ".png";
                ProcessImage.Save(MainWindow.IMGjustProcessed, ImageFormat.Png);
                MainWindow.ShowTransformIMG = true;
            }
            else if (MainWindow.aff_trans == true)
            {
                float tx = Convert.ToSingle(Aff_Tx.Text);
                float ty = Convert.ToSingle(Aff_Ty.Text);
                float sx = Convert.ToSingle(Aff_Scalex.Text);
                float sy = Convert.ToSingle(Aff_Scaley.Text);
                float rot = Convert.ToSingle(Aff_Rot.Text);
                float shear = Convert.ToSingle(Aff_Shear.Text);
                float tarx = 0, tary = 0;

                ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);

                AffineTransform Aff = new AffineTransform(3);

                Aff.Coeffs[0] = (float)(Math.Cos(rot)) * sx;             //(a1)
                Aff.Coeffs[1] = (float)(-Math.Sin(rot + shear)) * sy;    //(a2)
                Aff.Coeffs[2] = tx;                                      //(a0)
                Aff.Coeffs[3] = (float)(Math.Sin(rot)) * sx;             //(b1)  
                Aff.Coeffs[4] = (float)(Math.Cos(rot + shear)) * sy;     //(b2)
                Aff.Coeffs[5] = ty;                                      //(b0)

                for (int i = 0; i < OriImgW; i++)
                {
                    for (int j = 0; j < OriImgH; j++)
                    {
                        Aff.Transform(i - OriImgW / 2, -j + OriImgH / 2, ref tarx, ref tary);
                        if ((tarx + OriImgW / 2) > 0 && (-tary + OriImgH / 2) > 0 && (tarx + OriImgW / 2) < (OriImgW - 1) && (-tary + OriImgH / 2) < (OriImgH - 1))
                        {
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 1] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 1];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 2] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 2];
                        }
                    }
                }

                MainWindow.IMGjustProcessed = MainWindow.Savedir + "/Hw2_Front_Affine" + Convert.ToString(MainWindow.count) + ".png";
                ProcessImage.Save(MainWindow.IMGjustProcessed, ImageFormat.Png);
                MainWindow.ShowTransformIMG = true;
            }
            else if (MainWindow.proj_trans == true)
            {
                float tx = Convert.ToSingle(Proj_Tx.Text);
                float ty = Convert.ToSingle(Proj_Ty.Text);
                float tz = Convert.ToSingle(Proj_Tz.Text);
                float omega = (Convert.ToSingle(Proj_Omega.Text) * (float)(Math.PI)) / 180;
                float phi = Convert.ToSingle(Proj_Phi.Text) * (float)(Math.PI) / 180;  //phi
                float kappa = Convert.ToSingle(Proj_Kappa.Text) * (float)(Math.PI) / 180;  //kappa 
                float sx = Convert.ToSingle(Proj_Scalex.Text);
                float sy = Convert.ToSingle(Proj_Scaley.Text);
                float tarx = 0, tary = 0;

                //Projective_transformation
                ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);

                ProjectiveTransform Proj = new ProjectiveTransform(3);

                Proj.Coeffs[0] = (float)((Math.Cos(kappa) * Math.Cos(phi))) * sx / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[1] = (float)((-Math.Sin(kappa) * Math.Cos(omega) + Math.Cos(kappa) * Math.Sin(phi) * Math.Sin(omega))) * sx / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[2] = (float)(tx + (float)(Math.Sin(kappa) * Math.Sin(omega) + Math.Cos(kappa) * Math.Sin(phi) * Math.Cos(omega))) * sx / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[3] = (float)((Math.Sin(kappa) * Math.Cos(phi))) * sy / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[4] = (float)((Math.Cos(kappa) * Math.Cos(omega) + Math.Sin(kappa) * Math.Sin(phi) * Math.Sin(omega))) * sy / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[5] = (float)(ty + (float)(-Math.Cos(kappa) * Math.Sin(omega) + Math.Sin(kappa) * Math.Sin(phi) * Math.Cos(omega))) * sy / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[6] = (float)(-Math.Sin(phi)) / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                Proj.Coeffs[7] = (float)(Math.Cos(phi) * Math.Sin(omega)) / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));

                for (int i = 0; i < OriImgW; i++)
                {
                    for (int j = 0; j < OriImgH; j++)
                    {
                        Proj.Transform(i - OriImgW / 2, -j + OriImgH / 2, ref tarx, ref tary);
                        if ((tarx + OriImgW / 2) > 0 && (-tary + OriImgH / 2) > 0 && (tarx + OriImgW / 2) < (OriImgW - 1) && (-tary + OriImgH / 2) < (OriImgH - 1))
                        {
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 1] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 1];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 2] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 2];
                        }
                    }
                }

                MainWindow.IMGjustProcessed = MainWindow.Savedir + "/Hw2_Front_Projective" + Convert.ToString(MainWindow.count) + ".png";
                ProcessImage.Save(MainWindow.IMGjustProcessed, ImageFormat.Png);
                MainWindow.ShowTransformIMG = true;
            }
            this.Close();
        }

        private void PutResultat01_Checked(object sender, RoutedEventArgs e)
        {
            PutLeft = true;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Backward_Click(object sender, RoutedEventArgs e)
        {
            if (TransImg2.IsChecked == true) ImagefromMainwindow = MainWindow.OriginalImage2;

            if (MainWindow.sim_trans == true)
            {
                float tx = Convert.ToSingle(Sim_Tx.Text);
                float ty = Convert.ToSingle(Sim_Tx.Text);
                float rot = (Convert.ToSingle(Sim_Rot.Text) * (float)(Math.PI)) /180;
                float scale = Convert.ToSingle(Sim_Scale.Text);
                float orix = 0, oriy = 0;

                ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
                int d = ProcessImage.NumberOfBands;

                SimilarityTransform Sim = new SimilarityTransform(3);

                float a1 = (float)(Math.Cos(rot)) * scale; //(a1)
                float b1 = (float)(Math.Sin(rot)) * scale; //(b1)
                float a0 = tx; //(a0)
                float b0 = ty; //(b0)

                float value = (a1 * a1 + b1 * b1);
                Sim.Coeffs[0] = (float)a1 / value;
                Sim.Coeffs[1] = (float)-b1 / value;
                Sim.Coeffs[2] = (float)(-a1 * a0 - b1 * b0) / value;
                Sim.Coeffs[3] = (float)(a0 * b1 - a1 * b0) / value;

                for (int i = 0; i < OriImgW; i++)
                {
                    for (int j = 0; j < OriImgH; j++)
                    {
                        Sim.Transform(i - OriImgW / 2, -j + OriImgH / 2, ref orix, ref oriy);
                        for (int k = 0; k < d; k++)
                        {
                            if ((orix + OriImgW / 2) > 0 && (-oriy + OriImgH / 2) > 0 && (orix + OriImgW / 2) < (OriImgW - 2) && (-oriy + OriImgH / 2) < (OriImgH - 2))
                            {
                                ProcessImage.ByteData[j * OriImgW * d + i * d + k] = (byte)(Interpolation.Bicubic(ImagefromMainwindow, orix + OriImgW / 2, -oriy + OriImgH / 2, k));
                            }
                        }
                    }
                }

                MainWindow.IMGjustProcessed = MainWindow.Savedir + "/Hw2_Back_Similarity" + Convert.ToString(MainWindow.count) + ".png";
                ProcessImage.Save(MainWindow.IMGjustProcessed, ImageFormat.Png);
                MainWindow.ShowTransformIMG = true;
            }
            else if (MainWindow.aff_trans == true)
            {
                float tx = Convert.ToSingle(Aff_Tx.Text);
                float ty = Convert.ToSingle(Aff_Ty.Text);
                float sx = Convert.ToSingle(Aff_Scalex.Text);
                float sy = Convert.ToSingle(Aff_Scaley.Text);
                float rot = Convert.ToSingle(Aff_Rot.Text)*(float)(Math.PI)/180;
                float shear = Convert.ToSingle(Aff_Shear.Text);
                float orix = 0, oriy = 0;

                ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
                int d = ProcessImage.NumberOfBands;

                AffineTransform Aff = new AffineTransform(3);

                float a1 = (float)(Math.Cos(rot)) * sx;             //(a1)
                float a2 = (float)(-Math.Sin(rot + shear)) * sy;    //(a2)
                float a0 = tx;                                      //(a0)
                float b1 = (float)(Math.Sin(rot)) * sx;             //(b1)  
                float b2 = (float)(Math.Cos(rot + shear)) * sy;     //(b2)
                float b0 = ty;                                      //(b0)

                float value = (a2 * b1 - a1 * b2);
                Aff.Coeffs[0] = (float)-b2 / value;
                Aff.Coeffs[1] = (float)a2 / value;
                Aff.Coeffs[2] = (float)(a0 * b2 - a2 * b0) / value;
                Aff.Coeffs[3] = (float)b1 / value;
                Aff.Coeffs[4] = (float)-a1 / value;
                Aff.Coeffs[5] = (float)(a1 * b0 - a0 * b1) / value;

                for (int i = 0; i < OriImgW; i++)
                {
                    for (int j = 0; j < OriImgH; j++)
                    {
                        Aff.Transform(i - OriImgW / 2, -j + OriImgH / 2, ref orix, ref oriy);
                        for (int k = 0; k < d; k++)
                        {
                            if ((orix + OriImgW / 2) > 0 && (-oriy + OriImgH / 2) > 0 && (orix + OriImgW / 2) < (OriImgW - 2) && (-oriy + OriImgH / 2) < (OriImgH - 2))
                            {
                                ProcessImage.ByteData[j * OriImgW * d + i * d + k] = (byte)(Interpolation.Bicubic(ImagefromMainwindow, orix + OriImgW / 2, -oriy + OriImgH / 2, k));
                            }
                        }
                    }
                }

                MainWindow.IMGjustProcessed = MainWindow.Savedir + "/Hw2_Back_Affine" + Convert.ToString(MainWindow.count) + ".png";
                ProcessImage.Save(MainWindow.IMGjustProcessed, ImageFormat.Png);
                MainWindow.ShowTransformIMG = true;
            }
            else if (MainWindow.proj_trans == true)
            {
                float tx = Convert.ToSingle(Proj_Tx.Text);
                float ty = Convert.ToSingle(Proj_Ty.Text);
                float tz = Convert.ToSingle(Proj_Tz.Text);
                float omega = (Convert.ToSingle(Proj_Omega.Text) * (float)(Math.PI)) /180;
                float phi = Convert.ToSingle(Proj_Phi.Text) * (float)(Math.PI) /180;  //phi
                float kappa = Convert.ToSingle(Proj_Kappa.Text) * (float)(Math.PI) /180;  //kappa 
                float sx = Convert.ToSingle(Proj_Scalex.Text);
                float sy = Convert.ToSingle(Proj_Scaley.Text);
                float tarx = 0, tary = 0;

                //Projective_transformation
                ProcessImage = new LOCImage(OriImgW, OriImgH, OriImgDPIX, OriImgDPIY, PixelFormats.Bgr24, null);
                int d = ProcessImage.NumberOfBands;

                ProjectiveTransform Proj = new ProjectiveTransform(3);

                float a1 = (float)((Math.Cos(kappa) * Math.Cos(phi))) * sx / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float a2 = (float)((-Math.Sin(kappa) * Math.Cos(omega) + Math.Cos(kappa) * Math.Sin(phi) * Math.Sin(omega))) * sx / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float a0 = (float)(tx + (float)(Math.Sin(kappa) * Math.Sin(omega) + Math.Cos(kappa) * Math.Sin(phi) * Math.Cos(omega))) * sx / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float b1 = (float)((Math.Sin(kappa) * Math.Cos(phi))) * sy / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float b2 = (float)((Math.Cos(kappa) * Math.Cos(omega) + Math.Sin(kappa) * Math.Sin(phi) * Math.Sin(omega))) * sy / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float b0 = (float)(ty + (float)(-Math.Cos(kappa) * Math.Sin(omega) + Math.Sin(kappa) * Math.Sin(phi) * Math.Cos(omega))) * sy / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float c1 = (float)(-Math.Sin(phi)) / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));
                float c2 = (float)(Math.Cos(phi) * Math.Sin(omega)) / (tz + (float)(Math.Cos(phi) * Math.Cos(omega)));

                float value = (float)(a1 * b2 - a2 * b1);
                Proj.Coeffs[0] = (float)(b2 - b0 * c2) / value;
                Proj.Coeffs[1] = (float)(a0 * c2 - a2) / value;
                Proj.Coeffs[2] = (float)(a2 * b0 - a0 * b2) / value;
                Proj.Coeffs[3] = (float)(b0 * c1 - b1) / value;
                Proj.Coeffs[4] = (float)(a1 - a0 * c1) / value;
                Proj.Coeffs[5] = (float)(a0 * b1 - a1 * b0) / value;
                Proj.Coeffs[6] = (float)(b1 * c2 - b2 * c1) / value;
                Proj.Coeffs[7] = (float)(a2 * c1 - a1 * c2) / value;


                for (int i = 0; i < OriImgW; i++)
                {
                    for (int j = 0; j < OriImgH; j++)
                    {
                        Proj.Transform(i - OriImgW / 2, -j + OriImgH / 2, ref tarx, ref tary);
                        if ((tarx + OriImgW / 2) > 0 && (-tary + OriImgH / 2) > 0 && (tarx + OriImgW / 2) < (OriImgW - 1) && (-tary + OriImgH / 2) < (OriImgH - 1))
                        {
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 1] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 1];
                            ProcessImage.ByteData[(int)(-tary + OriImgH / 2) * OriImgW * 3 + (int)(tarx + OriImgW / 2) * 3 + 2] = ImagefromMainwindow.ByteData[j * OriImgW * 3 + i * 3 + 2];
                        }
                    }
                }

                MainWindow.IMGjustProcessed = MainWindow.Savedir + "/Hw2_Back_Projective" + Convert.ToString(MainWindow.count) + ".png";                
                ProcessImage.Save(MainWindow.IMGjustProcessed, ImageFormat.Png);
                MainWindow.ShowTransformIMG = true;
            }
            this.Close();
        }

    }

    
}
