using UnityEngine;

namespace Data
{
    public class DeviceCapabilities
    {
        public static string AdvertisementId { get; private set; }

        public static void InitAdvertisingId()
        {
            AdvertisementId = "00000000000000000000000000000000";

            var advertisementId = PlayerPrefs.GetString("advertisementId");

            if (string.IsNullOrEmpty(advertisementId))
            {
                Application.RequestAdvertisingIdentifierAsync((advertisingId, trackingEnabled, error) =>
                {
                    if (!string.IsNullOrEmpty(advertisingId))
                    {
                        AdvertisementId = advertisingId;
                        PlayerPrefs.SetString("advertisementId", advertisingId);
                    }
                });
            }
            else
            {
                AdvertisementId = advertisementId;
            }
        }
    }
}