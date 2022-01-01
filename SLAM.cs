using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using LOC.Photogrammetry;

namespace SLAM
{
    public class KeyFrame
    {
        private int kfid, frameid;
        private float xind, yind, zind, x97, y97, z97;
        private float[,] rot;
        private RotationMatrix rotm;
        private ExOrietation eo;

        public int KFID { get => kfid; set => kfid = value; }
        public int FrameID { get => frameid; set => frameid = value; }
        public float Xind { get => xind; set => xind = value; }
        public float Yind { get => yind; set => yind = value; }
        public float Zind { get => zind; set => zind = value; }
        public float X97 { get => x97; set => x97 = value; }
        public float Y97 { get => y97; set => y97 = value; }
        public float Z97 { get => z97; set => z97 = value; }
        public float[,] Rot { get => rot; set => rot = value; }
        public RotationMatrix RotM { get => rotm; set => rotm = value; }
        public ExOrietation EO { get => eo; set => eo = value; }

        public KeyFrame()
        {

        }

        public KeyFrame(int KFID, float Xind, float Yind, float Zind, float[,] Rot, int FrameID)
        {
            this.KFID = KFID;
            this.FrameID = FrameID;
            this.Xind = Xind;
            this.Yind = Yind;
            this.Zind = Zind;
            this.Rot = Rot;
        }

        public KeyFrame(int KFID, float Xind, float Yind, float Zind, RotationMatrix RotM, int FrameID)
        {
            this.KFID = KFID;
            this.FrameID = FrameID;
            this.Xind = Xind;
            this.Yind = Yind;
            this.Zind = Zind;
            this.RotM = RotM;
        }

        public KeyFrame(int KFID, ExOrietation EO, int FrameID)
        {
            this.KFID = KFID;
            this.FrameID = FrameID;
            this.EO = EO;

        }
    }

    public class KeyPoint
    {
        private float kfx, kfy;
        private int fid;
        private ImagePoints.Coordinate imgxy;

        public int FID { get => fid; set => fid = value; }
        public float KFX { get => kfx; set => kfx = value; }
        public float KFY { get => kfy; set => kfy = value; }
        public ImagePoints.Coordinate IMGXY { get => imgxy; set => imgxy = value; }

        public KeyPoint()
        { 
        
        }
        
        /*public KeyPoint(int kfid, float kfx, float kfy)
        {
            this.KFID = kfid;
            this.KFX = kfx;
            this.KFY = kfy;
        }*/

        public KeyPoint(int fid, float KFx, float KFy)
        {
            this.FID = fid;
            this.IMGXY = new ImagePoints.Coordinate(KFx, KFy);
            //this.IMGXY.X = KFx;
            //this.IMGXY.Y = KFy;
        }

    }

    public class ControlPoint
    {
        private string cpid;
        private float cpx97, cpy97, cpz97, cpxind, cpyind, cpzind;
        private List<KeyPoint> keypointlist;

        public string CPID { get => cpid; set => cpid = value;}
        public float CPX97 { get => cpx97; set => cpx97 = value; }
        public float CPY97 { get => cpy97; set => cpy97 = value; }
        public float CPZ97 { get => cpz97; set => cpz97 = value; }
        public float CPXind { get => cpxind; set => cpxind = value; }
        public float CPYind { get => cpyind; set => cpyind = value; }
        public float CPZind { get => cpzind; set => cpzind = value; }
        public List<KeyPoint> KeyPointList { get => keypointlist; set => keypointlist = value; }
        

        public ControlPoint()
        { 
        
        }

        public ControlPoint(string cpid, float cpx97, float cpy97, float cpz97)
        {
            this.CPID = cpid;
            this.CPX97 = cpx97;
            this.CPY97 = cpy97;
            this.CPZ97 = cpz97;
        }

        public ControlPoint(string cpid, float cpx97, float cpy97, float cpz97, List<KeyPoint> keypointlist)
        {
            this.CPID = cpid;
            this.CPX97 = cpx97;
            this.CPY97 = cpy97;
            this.CPZ97 = cpz97;
            this.KeyPointList = keypointlist;
        }

    }
    
}
