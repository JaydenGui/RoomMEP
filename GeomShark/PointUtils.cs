using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomShark
{
    public class PointUtils
    {
        public static bool IsPointBetweenOtherTwoPoints(double linePointStartX, double linePointStartY, 
                                    double linePointEndX, double linePointEndY, 
                                    double checkPointX, double checkPointY,
                                    int acccuracy)
        {
            if (checkPointX - Math.Max(linePointStartX, linePointEndX) > acccuracy ||
                    Math.Min(linePointStartX, linePointEndX) - checkPointX > acccuracy ||
                    checkPointY - Math.Max(linePointStartY, linePointEndY) > acccuracy ||
                    Math.Min(linePointStartY, linePointEndY) - checkPointY > acccuracy)
                return false;

            if (Math.Abs(linePointEndX - linePointStartX) < acccuracy)
                return Math.Abs(linePointStartX - checkPointX) < acccuracy || Math.Abs(linePointEndX - checkPointX) < acccuracy;
            if (Math.Abs(linePointEndY - linePointStartY) < acccuracy)
                return Math.Abs(linePointStartY - checkPointY) < acccuracy || Math.Abs(linePointEndY - checkPointY) < acccuracy;

            double x = linePointStartX + (checkPointY - linePointStartY) * (linePointEndX - linePointStartX) / (linePointEndY - linePointStartY);
            double y = linePointStartY + (checkPointX - linePointStartX) * (linePointEndY - linePointStartY) / (linePointEndX - linePointStartX);

            return Math.Abs(checkPointX - x) < acccuracy || Math.Abs(checkPointY - y) < acccuracy;
        }
    }
}
