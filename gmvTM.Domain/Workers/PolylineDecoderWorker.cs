using System;
using System.Collections.Generic;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers
{
    public sealed class PolylineDecoderWorker : IPolylineDecoderWorker
    {
        private static readonly double Factor = Math.Pow(10, 5);

        public List<CoordinatesViewItem> Decode(string encodedPolyline)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(encodedPolyline);

            List<CoordinatesViewItem> coordinates = new List<CoordinatesViewItem>();
            int index = 0;
            int latitude = 0;
            int longitude = 0;
            string polyline = encodedPolyline.Trim();

            while (index < polyline.Length)
            {
                latitude += DecodeNext(polyline, ref index);
                longitude += DecodeNext(polyline, ref index);
                coordinates.Add(new CoordinatesViewItem(latitude / Factor, longitude / Factor));
            }

            return coordinates;
        }

        //copied from claude
        private static int DecodeNext(string encoded, ref int index)
        {
            int result = 0;
            int shift = 0;
            int b;

            do
            {
                if (index >= encoded.Length)
                    throw new FormatException("Invalid encoded polyline.");

                b = encoded[index++] - 63;
                result |= (b & 0x1F) << shift;
                shift += 5;
            }
            while (b >= 0x20);

            return (result & 1) != 0 ? ~(result >> 1) : result >> 1;
        }
    }
}
