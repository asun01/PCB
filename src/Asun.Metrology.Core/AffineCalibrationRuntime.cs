using System.Security.Cryptography;
using System.Text;

namespace Asun.Metrology.Core;

public static class AffineCalibrationRuntime
{
    public static AffineCalibrationResult2D Fit(
        IEnumerable<CalibrationCorrespondence2D> correspondences,
        double determinantThreshold=1e-12)
    {
        ArgumentNullException.ThrowIfNull(correspondences);

        var points=correspondences
            .OrderBy(item=>item.Source.X)
            .ThenBy(item=>item.Source.Y)
            .ThenBy(item=>item.Target.X)
            .ThenBy(item=>item.Target.Y)
            .ToArray();

        if(points.Length<3)
            throw new ArgumentException(
                "Affine calibration requires at least three correspondences.",
                nameof(correspondences));

        if(points.Any(point=>!point.IsValid))
            throw new ArgumentException(
                "Affine calibration correspondences must be finite.",
                nameof(correspondences));

        if(!double.IsFinite(determinantThreshold) ||
           determinantThreshold<=0)
        {
            throw new ArgumentOutOfRangeException(nameof(determinantThreshold));
        }

        var normal=new double[6,6];
        var rhs=new double[6];

        foreach(var point in points)
        {
            var x=point.Source.X;
            var y=point.Source.Y;

            var rowX=new[]{x,y,1d,0d,0d,0d};
            var rowY=new[]{0d,0d,0d,x,y,1d};

            Accumulate(normal,rhs,rowX,point.Target.X);
            Accumulate(normal,rhs,rowY,point.Target.Y);
        }

        var coefficients=Solve(normal,rhs,determinantThreshold);

        var transform=new AffineTransform2D(
            coefficients[0],
            coefficients[1],
            coefficients[3],
            coefficients[4],
            coefficients[2],
            coefficients[5]);

        if(!AffineTransform2DValidationRuntime.IsValid(
            transform,
            determinantThreshold))
        {
            throw new InvalidOperationException(
                "Computed affine calibration transform is invalid.");
        }

        var squaredErrors=points.Select(point=>
        {
            var mapped=transform.Transform(point.Source);
            return mapped.DistanceSquaredTo(point.Target);
        }).ToArray();

        var rms=Math.Sqrt(
            squaredErrors.Average());

        var maximum=Math.Sqrt(
            squaredErrors.Max());

        var canonical=new StringBuilder();
        canonical.Append(transform.M11.ToString("R")).Append('|')
            .Append(transform.M12.ToString("R")).Append('|')
            .Append(transform.M21.ToString("R")).Append('|')
            .Append(transform.M22.ToString("R")).Append('|')
            .Append(transform.Tx.ToString("R")).Append('|')
            .Append(transform.Ty.ToString("R")).Append('|')
            .Append(points.Length).Append('|')
            .Append(rms.ToString("R")).Append('|')
            .Append(maximum.ToString("R"));

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();

        return new AffineCalibrationResult2D(
            transform,
            points.Length,
            rms,
            maximum,
            fingerprint);
    }

    private static void Accumulate(
        double[,] normal,
        double[] rhs,
        double[] row,
        double target)
    {
        for(var rowIndex=0;rowIndex<6;rowIndex++)
        {
            rhs[rowIndex]+=row[rowIndex]*target;

            for(var columnIndex=0;columnIndex<6;columnIndex++)
            {
                normal[rowIndex,columnIndex]+=
                    row[rowIndex]*row[columnIndex];
            }
        }
    }

    private static double[] Solve(
        double[,] matrix,
        double[] rhs,
        double threshold)
    {
        var size=rhs.Length;
        var a=new double[size,size+1];

        for(var row=0;row<size;row++)
        {
            for(var column=0;column<size;column++)
                a[row,column]=matrix[row,column];

            a[row,size]=rhs[row];
        }

        for(var pivot=0;pivot<size;pivot++)
        {
            var pivotRow=pivot;
            var pivotMagnitude=Math.Abs(a[pivot,pivot]);

            for(var row=pivot+1;row<size;row++)
            {
                var candidate=Math.Abs(a[row,pivot]);

                if(candidate>pivotMagnitude)
                {
                    pivotMagnitude=candidate;
                    pivotRow=row;
                }
            }

            if(!double.IsFinite(pivotMagnitude) ||
               pivotMagnitude<threshold)
            {
                throw new ArgumentException(
                    "Affine calibration correspondences are singular or insufficiently constrained.");
            }

            if(pivotRow!=pivot)
            {
                for(var column=pivot;column<=size;column++)
                    (a[pivot,column],a[pivotRow,column])=
                        (a[pivotRow,column],a[pivot,column]);
            }

            var divisor=a[pivot,pivot];

            for(var column=pivot;column<=size;column++)
                a[pivot,column]/=divisor;

            for(var row=0;row<size;row++)
            {
                if(row==pivot)
                    continue;

                var factor=a[row,pivot];

                if(Math.Abs(factor)<threshold)
                {
                    a[row,pivot]=0;
                    continue;
                }

                for(var column=pivot;column<=size;column++)
                    a[row,column]-=factor*a[pivot,column];
            }
        }

        var solution=new double[size];

        for(var row=0;row<size;row++)
            solution[row]=a[row,size];

        return solution;
    }
}
