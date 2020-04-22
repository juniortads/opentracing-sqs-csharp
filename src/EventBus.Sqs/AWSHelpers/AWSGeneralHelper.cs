using System;
using Amazon;

namespace EventBus.Sqs.AWSHelpers
{
    internal class AWSGeneralHelper
    {
        internal static RegionEndpoint GetRegionEndpoint(string RegionName = "USEast1")
        {
            RegionEndpoint result = null;

            RegionName = RegionName.Replace("-", "").ToLowerInvariant();

            switch (RegionName)
            {
                case "apnortheast1": { result = RegionEndpoint.APNortheast1; break; }
                case "apnortheast2": { result = RegionEndpoint.APNortheast2; break; }
                case "apnortheast3": { result = RegionEndpoint.APNortheast3; break; }
                case "apsoutheast1": { result = RegionEndpoint.APSoutheast1; break; }
                case "apsoutheast2": { result = RegionEndpoint.APSoutheast2; break; }
                case "cnnorth1": { result = RegionEndpoint.CNNorth1; break; }
                case "eucentral1": { result = RegionEndpoint.EUCentral1; break; }
                case "euwest1": { result = RegionEndpoint.EUWest1; break; }
                case "euwest2": { result = RegionEndpoint.EUWest2; break; }
                case "euwest3": { result = RegionEndpoint.EUWest3; break; }
                case "saeast1": { result = RegionEndpoint.SAEast1; break; }
                case "useast1": { result = RegionEndpoint.USEast1; break; }
                case "useast2": { result = RegionEndpoint.USEast2; break; }
                case "usgovcloudwest1": { result = RegionEndpoint.USGovCloudWest1; break; }
                case "uswest1": { result = RegionEndpoint.USWest1; break; }
                case "uswest2": { result = RegionEndpoint.USWest2; break; }
                case "apeast1": { result = RegionEndpoint.APEast1; break; }
                case "apsouth1":{ result = RegionEndpoint.APSouth1; break; }
                case "cacentral1": { result = RegionEndpoint.CACentral1; break; }
                case "cnnorthwest1": { result = RegionEndpoint.CNNorthWest1; break; }
                case "eunorth1": { result = RegionEndpoint.EUNorth1; break; }
                case "usgoveast1": { result = RegionEndpoint.USGovCloudEast1; break; }
            }

            return result;
        }
    }
}
