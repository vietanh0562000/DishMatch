using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    public static class AdditionalResolutions
    {
        // Using parenthesis in a size view name is not recommended as it will mess with the parser
        // Similarly re-using the same name as another size will cause the second size to be skipped as names must be unique (even if sizes are different)

        // Promotional values from:
        // https://makeawebsitehub.com/social-media-image-sizes-cheat-sheet/
        // https://www.agorapulse.com/blog/all-twitter-image-sizes-best-practices

        // Steam values from:
        // https://partner.steamgames.com/doc/store/assets

        // iOS values from: 
        // https://developer.apple.com/library/content/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html

        public static Dictionary<Resolution, Rect> safeAreaForResolution = new Dictionary<Resolution, Rect>()
        {
            // iPhone X, Xs
            { new Resolution { width = 1125, height = 2436 }, new Rect (0f, 102f, 1125f, 2202f) },
            { new Resolution { width = 2436, height = 1125 }, new Rect (132f, 63f, 2172f, 1062f) },

            // iPhone Xs Max
            { new Resolution { width = 1242, height = 2688 }, new Rect (0f, 68f, 1242f, 2532f) },
            { new Resolution { width = 2688, height = 1242 }, new Rect (132f, 63f, 2424f, 1179f) },

            // iPhone Xr
            { new Resolution { width = 828, height = 1792 }, new Rect (0f, 68f, 828f, 1636f) },
            { new Resolution { width = 1792, height = 828 }, new Rect (88f, 42f, 1616f, 786f) },

            // Pixel 3 XL
            //{ new Resolution { width = 1440, height = 2960 }, new Rect (0f, 0f, 1440f, 2960f) },
            //{ new Resolution { width = 2960, height = 1440 }, new Rect (0f, 0f, 2960f, 1440f) },
        };

        public static Dictionary<Resolution, Resolution> resolutionForAspectRatio = new Dictionary<Resolution, Resolution>()
        {
            { new Resolution { width = 5, height = 4 }, new Resolution { width = 1280, height = 1024 } },
            { new Resolution { width = 4, height = 5 }, new Resolution { width = 1024, height = 1280 } },

            { new Resolution { width = 4, height = 3 }, new Resolution { width = 640, height = 480 } },
            { new Resolution { width = 3, height = 4 }, new Resolution { width = 480, height = 640 } },

            { new Resolution { width = 2, height = 3 }, new Resolution { width = 640, height = 960 } },
            { new Resolution { width = 3, height = 2 }, new Resolution { width = 960, height = 640 } },

            { new Resolution { width = 16, height = 10 }, new Resolution { width = 1280, height = 800 } },
            { new Resolution { width = 10, height = 16 }, new Resolution { width = 800, height = 1280 } },

            { new Resolution { width = 16, height = 9 }, new Resolution { width = 1920, height = 1080 } },
            { new Resolution { width = 9, height = 16 }, new Resolution { width = 1080, height = 1920 } }
        };

#if UNITY_EDITOR

        public static Dictionary<UnityEditor.GameViewSizeGroupType, Dictionary<string, Resolution>> forGroupType = new Dictionary<UnityEditor.GameViewSizeGroupType, Dictionary<string, Resolution>>()
        {
            { UnityEditor.GameViewSizeGroupType.Android, new Dictionary<string, Resolution>() {
                    { "Promotional/Android/Feature Graphic", new Resolution { width = 1024, height = 500 } },
                    { "Promotional/Android/Promo Graphic", new Resolution { width = 180, height = 120 } },
                    { "Promotional/Android/TV Banner", new Resolution { width = 1280, height = 720 } },

                    { "Promotional/Amazon/Promo Image", new Resolution { width = 1024, height = 500 } },
                    { "Promotional/Amazon/Icon Large", new Resolution { width = 512, height = 512 } },
                    { "Promotional/Amazon/Icon Small", new Resolution { width = 114, height = 114 } },

                    { "Promotional/Android/Icon", new Resolution { width = 512, height = 512 } },
                    { "Promotional/Android/Feature", new Resolution { width = 1024, height = 500 } },
                    { "Promotional/Android/Banner", new Resolution { width = 320, height = 180 } },

                    // Icons
                    { "Android/Icon/Adaptive Icon/xxxhdpi", new Resolution { width = 432, height = 432 } },
                    { "Android/Icon/Adaptive Icon/xxhdpi", new Resolution { width = 324, height = 324 } },
                    { "Android/Icon/Adaptive Icon/xhdpi", new Resolution { width = 216, height = 216 } },
                    { "Android/Icon/Adaptive Icon/hdpi", new Resolution { width = 162, height = 162 } },
                    { "Android/Icon/Adaptive Icon/mdpi", new Resolution { width = 108, height = 108 } },
                    { "Android/Icon/Adaptive Icon/ldpi", new Resolution { width = 81, height = 81 } },

                    { "Android/Icon/Round Icon/xxxhdpi", new Resolution { width = 192, height = 192 } },
                    { "Android/Icon/Round Icon/xxhdpi", new Resolution { width = 144, height = 144 } },
                    { "Android/Icon/Round Icon/xhdpi", new Resolution { width = 96, height = 96 } },
                    { "Android/Icon/Round Icon/hdpi", new Resolution { width = 72, height = 72 } },
                    { "Android/Icon/Round Icon/mdpi", new Resolution { width = 48, height = 48 } },
                    { "Android/Icon/Round Icon/ldpi", new Resolution { width = 36, height = 36 } },

                    { "Android/Icon/Legacy Icon/xxxhdpi", new Resolution { width = 192, height = 192 } },
                    { "Android/Icon/Legacy Icon/xxhdpi", new Resolution { width = 144, height = 144 } },
                    { "Android/Icon/Legacy Icon/xhdpi", new Resolution { width = 96, height = 96 } },
                    { "Android/Icon/Legacy Icon/hdpi", new Resolution { width = 72, height = 72 } },
                    { "Android/Icon/Legacy Icon/mdpi", new Resolution { width = 48, height = 48 } },
                    { "Android/Icon/Legacy Icon/ldpi", new Resolution { width = 36, height = 36 } },

                    // S8 or Note 8
                    { "Android/Samsung Tall", new Resolution { width = 1440, height = 2960 } },
                    { "Android/Samsung Wide", new Resolution { width = 2960, height = 1440 } },

                    { "Android/Standard Tall", new Resolution { width = 1080, height = 1920 } },
                    { "Android/Standard Wide", new Resolution { width = 1920, height = 1080 } },

                    // Pixel 3
                    { "Android/Pixel/3 Tall", new Resolution { width = 1080, height = 2160 } },
                    { "Android/Pixel/3 Wide", new Resolution { width = 2160, height = 1080 } },

                    // Pixel 3a
                    { "Android/Pixel/3a Tall", new Resolution { width = 1080, height = 2220 } },
                    { "Android/Pixel/3a Wide", new Resolution { width = 2220, height = 1080 } },

                    // Pixel 3 XL
                    { "Android/Pixel/3XL Tall", new Resolution { width = 1440, height = 2960 } },
                    { "Android/Pixel/3XL Wide", new Resolution { width = 2960, height = 1440 } },

                    // Pixel 4
                    { "Android/Pixel/4 Tall", new Resolution { width = 1080, height = 2280 } },
                    { "Android/Pixel/4 Wide", new Resolution { width = 2280, height = 1080 } },

                    // Pixel 4 XL
                    { "Android/Pixel/4XL Tall", new Resolution { width = 1440, height = 3040 } },
                    { "Android/Pixel/4XL Wide", new Resolution { width = 3040, height = 1440 } },

                    // Pixel 5
                    { "Android/Pixel/5 Tall", new Resolution { width = 1080, height = 2340 } },
                    { "Android/Pixel/5 Wide", new Resolution { width = 2340, height = 1080 } },

                    // Pixel 5 XL
                    { "Android/Pixel/5XL Tall", new Resolution { width = 1440, height = 2960 } },
                    { "Android/Pixel/5XL Wide", new Resolution { width = 2960, height = 1440 } },

                    // Pixel 6
                    { "Android/Pixel/6 Tall", new Resolution { width = 1080, height = 2400 } },
                    { "Android/Pixel/6 Wide", new Resolution { width = 2400, height = 1080 } },

                    // Pixel 6 Pro
                    { "Android/Pixel/6 Pro Tall", new Resolution { width = 1440, height = 3120 } },
                    { "Android/Pixel/6 Pro Wide", new Resolution { width = 3120, height = 1440 } },

                    // V30 or G6
                    { "Android/LG Tall", new Resolution { width = 1440, height = 2880 } },
                    { "Android/LG Wide", new Resolution { width = 2880, height = 1440 } },

                    { "Android Tablet/7\" Tall", new Resolution { width = 600, height = 1024 } },
                    { "Android Tablet/7\" Wide", new Resolution { width = 1024, height = 600 } },
                    { "Android Tablet/7\" or 10\" Tall", new Resolution { width = 800, height = 1280 } },
                    { "Android Tablet/7\" or 10\" Wide", new Resolution { width = 1280, height = 800 } },
                    { "Android Tablet/10\" Tall", new Resolution { width = 1200, height = 1920 } },
                    { "Android Tablet/10\" Wide", new Resolution { width = 1920, height = 1200 } },

                    // Amazon
                    { "Amazon/Fire HD 10", new Resolution { width = 1920, height = 1200 } },
                    { "Amazon/Fire HD 8", new Resolution { width = 1280, height = 800 } },
                    { "Amazon/Fire 7", new Resolution { width = 1024, height = 600 } },
                }
            },
            { UnityEditor.GameViewSizeGroupType.iOS, new Dictionary<string, Resolution>() {
                    { "Promotional/iOS/App Store Icon", new Resolution { width = 1024, height = 1024 } },

                    //Icons
                    { "iPhone/Icon/Application Icon/iPhone 60pt@3x", new Resolution { width = 180, height = 180 } },
                    { "iPhone/Icon/Application Icon/iPhone 60pt@2x", new Resolution { width = 120, height = 120 } },

                    { "iPad/Icon/Application Icon/iPad Pro 83.5pt@2x", new Resolution { width = 167, height = 167 } },
                    { "iPad/Icon/Application Icon/iPad 76pt@2x", new Resolution { width = 152, height = 152 } },
                    { "iPad/Icon/Application Icon/iPad 76pt@1x", new Resolution { width = 76, height = 76 } },

                    { "iPhone/Icon/Spotlight Icon/iPhone 40pt@3x", new Resolution { width = 120, height = 120 } },
                    { "iPhone/Icon/Spotlight Icon/iPhone 40pt@2x", new Resolution { width = 80, height = 80 } },

                    { "iPad/Icon/Spotlight Icon/iPad 40pt@2x", new Resolution { width = 80, height = 80 } },
                    { "iPad/Icon/Spotlight Icon/iPad 40pt@1x", new Resolution { width = 40, height = 40 } },

                    { "iPhone/Icon/Settings Icon/iPhone 29pt@3x", new Resolution { width = 87, height = 87 } },
                    { "iPhone/Icon/Settings Icon/iPhone 29pt@2x", new Resolution { width = 58, height = 58 } },
                    { "iPhone/Icon/Settings Icon/iPhone 29pt@1x", new Resolution { width = 29, height = 29 } },

                    { "iPad/Icon/Settings Icon/iPad 29pt@2x", new Resolution { width = 58, height = 58 } },
                    { "iPad/Icon/Settings Icon/iPad 29pt@1x", new Resolution { width = 29, height = 29 } },

                    { "iPhone/Icon/Notifications Icon/iPhone 20pt@3x", new Resolution { width = 60, height = 60 } },
                    { "iPhone/Icon/Notifications Icon/iPhone 20pt@2x", new Resolution { width = 40, height = 40 } },

                    { "iPad/Icon/Notifications Icon/iPad 20pt@2x", new Resolution { width = 40, height = 40 } },
                    { "iPad/Icon/Notifications Icon/iPad 20pt@1x", new Resolution { width = 20, height = 20 } },

                    { "iPhone/Screenshot/6.7\" Display Tall", new Resolution { width = 1290, height = 2796 } },
                    { "iPhone/Screenshot/6.7\" Display Wide", new Resolution { width = 2796, height = 1290 } },
                    { "iPhone/Screenshot/6.5\" Display Tall", new Resolution { width = 1284, height = 2778 } },
                    { "iPhone/Screenshot/6.5\" Display Wide", new Resolution { width = 2778, height = 1284 } },
                    { "iPhone/Screenshot/6.1\" Display Tall", new Resolution { width = 1179, height = 2556 } },
                    { "iPhone/Screenshot/6.1\" Display Wide", new Resolution { width = 2556, height = 1179 } },
                    { "iPhone/Screenshot/5.5\" Display Tall", new Resolution { width = 1242, height = 2208 } },
                    { "iPhone/Screenshot/5.5\" Display Wide", new Resolution { width = 2208, height = 1242 } },

                    { "iPhone/Screenshot/More/6.5\" Alt Display Tall", new Resolution { width = 1242, height = 2688 } },
                    { "iPhone/Screenshot/More/6.5\" Alt Display Wide", new Resolution { width = 2688, height = 1242 } },
                    { "iPhone/Screenshot/More/5.8\" Display Tall", new Resolution { width = 1170, height = 2532 } },
                    { "iPhone/Screenshot/More/5.8\" Display Wide", new Resolution { width = 2532, height = 1170 } },
                    { "iPhone/Screenshot/More/5.8\" Alt Display Tall", new Resolution { width = 1125, height = 2436 } },
                    { "iPhone/Screenshot/More/5.8\" Alt Display Wide", new Resolution { width = 2436, height = 1125 } },
                    { "iPhone/Screenshot/More/5.8\" Alt 1080 Display Tall", new Resolution { width = 1080, height = 2340 } },
                    { "iPhone/Screenshot/More/5.8\" Alt 1080 Display Wide", new Resolution { width = 2340, height = 1080 } },
                    { "iPhone/Screenshot/More/4.7\" Display Tall", new Resolution { width = 750, height = 1334 } },
                    { "iPhone/Screenshot/More/4.7\" Display Wide", new Resolution { width = 1334, height = 750 } },
                    { "iPhone/Screenshot/More/4\" with Status Bar Display Tall", new Resolution { width = 640, height = 1136 } },
                    { "iPhone/Screenshot/More/4\" with Status Bar Display Wide", new Resolution { width = 1136, height = 640 } },
                    { "iPhone/Screenshot/More/4\" without Status Bar Display Tall", new Resolution { width = 640, height = 1096 } },
                    { "iPhone/Screenshot/More/4\" without Status Bar Display Wide", new Resolution { width = 1136, height = 600 } },
                    { "iPhone/Screenshot/More/3.5\" with Status Bar Display Tall", new Resolution { width = 640, height = 960 } },
                    { "iPhone/Screenshot/More/3.5\" with Status Bar Display Wide", new Resolution { width = 960, height = 640 } },
                    { "iPhone/Screenshot/More/3.5\" without Status Bar Display Tall", new Resolution { width = 640, height = 920 } },
                    { "iPhone/Screenshot/More/3.5\" without Status Bar Display Wide", new Resolution { width = 960, height = 600 } },

                    { "iPhone/14 Pro Max (6.7\") Tall", new Resolution { width = 1290, height = 2796 } },
                    { "iPhone/14 Pro Max (6.7\") Wide", new Resolution { width = 2796, height = 1290 } },
                    { "iPhone/14 Pro (6.1\") Tall", new Resolution { width = 1179, height = 2556 } },
                    { "iPhone/14 Pro (6.1\") Wide", new Resolution { width = 2556, height = 1179 } },
                    { "iPhone/14 Plus (6.5\") Tall", new Resolution { width = 1284, height = 2778 } },
                    { "iPhone/14 Plus (6.5\") Wide", new Resolution { width = 2778, height = 1284 } },
                    { "iPhone/14 (5.8\") Tall", new Resolution { width = 1170, height = 2532 } },
                    { "iPhone/14 (5.8\") Wide", new Resolution { width = 2532, height = 1170 } },

                    { "iPhone/12-13 Pro Max (6.5\") Tall", new Resolution { width = 1284, height = 2778 } },
                    { "iPhone/12-13 Pro Max (6.5\") Wide", new Resolution { width = 2778, height = 1284 } },
                    { "iPhone/12-13 (Pro) (5.8\") Tall", new Resolution { width = 1170, height = 2532 } },
                    { "iPhone/12-13 (Pro) (5.8\") Wide", new Resolution { width = 2532, height = 1170 } },
                    { "iPhone/12-13 mini (5.8\") Tall", new Resolution { width = 1080, height = 2340 } },
                    { "iPhone/12-13 mini (5.8\") Wide", new Resolution { width = 2340, height = 1080 } },

                    { "iPhone/Xs Max (6.5\") Tall", new Resolution { width = 1242, height = 2688 } },
                    { "iPhone/Xs Max (6.5\") Wide", new Resolution { width = 2688, height = 1242 } },
                    { "iPhone/Xs (5.8\") Tall", new Resolution { width = 1125, height = 2436 } },
                    { "iPhone/Xs (5.8\") Wide", new Resolution { width = 2436, height = 1125 } },
                    { "iPhone/Xr (6.5\") Tall", new Resolution { width = 828, height = 1792 } },
                    { "iPhone/Xr (6.5\") Wide", new Resolution { width = 1792, height = 828 } },

                    { "iPhone/6-8 Plus (5.5\") Tall", new Resolution { width = 1080, height = 1920 } },
                    { "iPhone/6-8 Plus (5.5\") Wide", new Resolution { width = 1920, height = 1080 } },
                    { "iPhone/6-8 (4.7\") Tall", new Resolution { width = 750, height = 1334 } },
                    { "iPhone/6-8 (4.7\") Wide", new Resolution { width = 1334, height = 750 } },

                    { "iPhone/SE 2-3 gen (4.7\") Tall", new Resolution { width = 750, height = 1334 } },
                    { "iPhone/SE 2-3 gen (4.7\") Wide", new Resolution { width = 1334, height = 750 } },
                    { "iPhone/SE 1st gen (4\") Tall", new Resolution { width = 640, height = 1136 } },
                    { "iPhone/SE 1st gen (4\") Wide", new Resolution { width = 1136, height = 640 } },

                    { "iPad/Screenshot/12.9\" Display Tall", new Resolution { width = 2048, height = 2732 } },
                    { "iPad/Screenshot/12.9\" Display Wide", new Resolution { width = 2732, height = 2048 } },

                    { "iPad/Screenshot/More/11\" Display Tall", new Resolution { width = 1488, height = 2266 } },
                    { "iPad/Screenshot/More/11\" Display Wide", new Resolution { width = 2266, height = 1488 } },
                    { "iPad/Screenshot/More/11\" Alt Display Tall", new Resolution { width = 1668, height = 2388 } },
                    { "iPad/Screenshot/More/11\" Alt Display Wide", new Resolution { width = 2388, height = 1668 } },
                    { "iPad/Screenshot/More/11\" Alt 1640 Display Tall", new Resolution { width = 1640, height = 2360 } },
                    { "iPad/Screenshot/More/11\" Alt 1640 Display Wide", new Resolution { width = 2360, height = 1640 } },

                    { "iPad/Screenshot/More/10.5\" Display Tall", new Resolution { width = 1668, height = 2224 } },
                    { "iPad/Screenshot/More/10.5\" Display Wide", new Resolution { width = 2224, height = 1668 } },

                    { "iPad/Screenshot/More/9.7\" with Status Bar Display Tall", new Resolution { width = 1536, height = 2048 } },
                    { "iPad/Screenshot/More/9.7\" with Status Bar Display Wide", new Resolution { width = 2048, height = 1536 } },
                    { "iPad/Screenshot/More/9.7\" without Status Bar Display Tall", new Resolution { width = 1536, height = 2008 } },
                    { "iPad/Screenshot/More/9.7\" without Status Bar Display Wide", new Resolution { width = 2048, height = 1496 } },

                    { "iPad/Screenshot/More/9.7\" Alt with Status Bar Display Tall", new Resolution { width = 768, height = 1024 } },
                    { "iPad/Screenshot/More/9.7\" Alt with Status Bar Display Wide", new Resolution { width = 1024, height = 768 } },
                    { "iPad/Screenshot/More/9.7\" Alt without Status Bar Display Tall", new Resolution { width = 768, height = 1004 } },
                    { "iPad/Screenshot/More/9.7\" Alt without Status Bar Display Wide", new Resolution { width = 1024, height = 748 } },

                    { "iPad/Pro 12.9\" Tall", new Resolution { width = 2048, height = 2732 } },
                    { "iPad/Pro 12.9\" Wide", new Resolution { width = 2732, height = 2048 } },

                    { "iPad/Pro 11\" Tall", new Resolution { width = 1668, height = 2388 } },
                    { "iPad/Pro 11\" Wide", new Resolution { width = 2388, height = 1668 } },

                    { "iPad/Pro 10.5\" Tall", new Resolution { width = 1668, height = 2224 } },
                    { "iPad/Pro 10.5\" Wide", new Resolution { width = 2224, height = 1668 } },

                    { "iPad/Pro 10.2\" Tall", new Resolution { width = 1620, height = 2160 } },
                    { "iPad/Pro 10.2\" Wide", new Resolution { width = 2160, height = 1620 } },

                    { "iPad/Mini 6 Tall", new Resolution { width = 1536, height = 2048 } },
                    { "iPad/Mini 6 Wide", new Resolution { width = 2048, height = 1536 } },

                    { "iPad/Air 2, Mini 2-5 Tall", new Resolution { width = 1488, height = 2266 } },
                    { "iPad/Air 2, Mini 2-5 Wide", new Resolution { width = 2266, height = 1488 } },
                }
            }
        };

        public static Dictionary<string, Resolution> promotional = new Dictionary<string, Resolution>()
        {
            { "Promotional/Presskit/Header", new Resolution { width = 1200, height = 240 } },

            { "Promotional/Twitter/Icon", new Resolution { width = 73, height = 73 } },
            { "Promotional/Twitter/Header", new Resolution { width = 1500, height = 500 } },
            { "Promotional/Twitter/Visible Header", new Resolution { width = 1263, height = 421 } },
            { "Promotional/Twitter/Profile Photo", new Resolution { width = 400, height = 400 } },
            { "Promotional/Twitter/Post Photo", new Resolution { width = 1024, height = 512 } },
            { "Promotional/Twitter/In-Stream Photo", new Resolution { width = 440, height = 220 } },

            { "Promotional/YouTube/Thumbnail", new Resolution { width = 1280, height = 720 } },
            { "Promotional/YouTube/Channel Cover Photo", new Resolution { width = 2048, height = 1152 } },
            { "Promotional/YouTube/Channel Profile", new Resolution { width = 800, height = 800 } },

            { "Promotional/Unity Asset Store/Icon", new Resolution { width = 160, height = 160 } },
            { "Promotional/Unity Asset Store/Card Image", new Resolution { width = 420, height = 280 } },
            { "Promotional/Unity Asset Store/Cover Image", new Resolution { width = 1950, height = 1300 } },
            { "Promotional/Unity Asset Store/Social Media Image", new Resolution { width = 1200, height = 630 } },
            { "Promotional/Unity Asset Store/Screenshot", new Resolution { width = 2400, height = 1600 } },

            { "Promotional/Steam/Screenshot (720)", new Resolution { width = 1280, height = 720 } },
            { "Promotional/Steam/Screenshot (1080)", new Resolution { width = 1920, height = 1080 } },

            { "Promotional/Steam/Page Background", new Resolution { width = 1438, height = 1810 } },
            { "Promotional/Steam/Bundle Header", new Resolution { width = 707, height = 232 } },

            { "Promotional/Steam/Capsule/Header", new Resolution { width = 460, height = 215 } },
            { "Promotional/Steam/Capsule/Small", new Resolution { width = 231, height = 87 } },
            { "Promotional/Steam/Capsule/Main", new Resolution { width = 616, height = 353 } },
            { "Promotional/Steam/Capsule/Hero", new Resolution { width = 374, height = 448 } },
            { "Promotional/Steam/Capsule/Screenshots", new Resolution { width = 1920, height = 1080 } },

            { "Promotional/Steam/Community/Icon", new Resolution { width = 32, height = 32 } },
            { "Promotional/Steam/Community/Capsule", new Resolution { width = 184, height = 69 } },

            { "Promotional/Steam/Library/Capsule", new Resolution { width = 600, height = 900 } },
            { "Promotional/Steam/Library/Hero", new Resolution { width = 3840, height = 1240 } },
            { "Promotional/Steam/Library/Logo", new Resolution { width = 1280, height = 720 } },

            { "Promotional/Steam/Event/Cover", new Resolution { width = 800, height = 450 } },
            { "Promotional/Steam/Event/Header", new Resolution { width = 1920, height = 622 } },
            { "Promotional/Steam/Event/Spotlight", new Resolution { width = 2108, height = 460 } },

            { "Promotional/Oculus/Cover/Landscape", new Resolution { width = 2560, height = 1440 } },
            { "Promotional/Oculus/Cover/Square", new Resolution { width = 1440, height = 1440 } },
            { "Promotional/Oculus/Cover/Portrait", new Resolution { width = 1008, height = 1440 } },
            { "Promotional/Oculus/Cover/Mini", new Resolution { width = 1080, height = 360 } },

            { "Promotional/Oculus/Hero", new Resolution { width = 3000, height = 900 } },
            { "Promotional/Oculus/Logo", new Resolution { width = 1440, height = 9000 } },
            { "Promotional/Oculus/Screenshot", new Resolution { width = 2560, height = 1440 } },
            { "Promotional/Oculus/Trailer Cover", new Resolution { width = 2560, height = 1440 } },

            { "Promotional/Oculus/Icon/PC/1", new Resolution { width = 256, height = 256 } },
            { "Promotional/Oculus/Icon/PC/2", new Resolution { width = 96, height = 96 } },
            { "Promotional/Oculus/Icon/PC/3", new Resolution { width = 64, height = 256 } },
            { "Promotional/Oculus/Icon/PC/4", new Resolution { width = 48, height = 48 } },
            { "Promotional/Oculus/Icon/PC/5", new Resolution { width = 32, height = 32 } },
            { "Promotional/Oculus/Icon/PC/6", new Resolution { width = 16, height = 16 } },
            { "Promotional/Oculus/Icon/Mobile", new Resolution { width = 512, height = 512 } },

            { "Promotional/Itch/Cover", new Resolution { width = 315, height = 250 } },
            { "Promotional/Itch/Screenshot (Any)", new Resolution { width = 0, height = 0 } },

            { "Promotional/Kartridge/Icon", new Resolution { width = 500, height = 400 } },
            { "Promotional/Kartridge/Background", new Resolution { width = 1280, height = 720 } },
            { "Promotional/Kartridge/Screenshot", new Resolution { width = 1920, height = 1080 } },

            { "Promotional/Discord/Icon", new Resolution { width = 512, height = 512 } },

            { "Promotional/Twitch/Icon", new Resolution { width = 256, height = 256 } },
            { "Promotional/Twitch/Banner", new Resolution { width = 1200, height = 380 } },
        };

        public static Dictionary<string, Resolution> group = new Dictionary<string, Resolution>()
        {
            { "Group/App Store/Both Platforms", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Both Platforms Tall", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Both Platforms Wide", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/iOS", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/iOS Tall", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/iOS Wide", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/iOS Icons", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Android", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Android Tall", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Android Wide", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Android Icons", new Resolution { width = -1, height = -1 } },
            { "Group/App Store/Amazon", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Twitter", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/YouTube", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Unity Asset Store", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Steam", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Oculus", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Itch", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Kartridge", new Resolution { width = -1, height = -1 } },
            { "Group/Promotional/Twitch", new Resolution { width = -1, height = -1 } },
        };

        public static Dictionary<string, string[]> resolutionGroup = new Dictionary<string, string[]>()
        {
            { "Group/App Store/Android Icons", new string[] {
                "Promotional/Android/Icon",
                "Promotional/Android/Feature",

                    "Android/Icon/Adaptive Icon/xxxhdpi",  "Android/Icon/Adaptive Icon/xxhdpi", "Android/Icon/Adaptive Icon/xhdpi",
                    "Android/Icon/Adaptive Icon/hdpi", "Android/Icon/Adaptive Icon/mdpi", "Android/Icon/Adaptive Icon/ldpi",

                    "Android/Icon/Round Icon/xxxhdpi", "Android/Icon/Round Icon/xxhdpi", "Android/Icon/Round Icon/xhdpi",
                    "Android/Icon/Round Icon/hdpi", "Android/Icon/Round Icon/mdpi", "Android/Icon/Round Icon/ldpi",

                    "Android/Icon/Legacy Icon/xxxhdpi", "Android/Icon/Legacy Icon/xxhdpi", "Android/Icon/Legacy Icon/xhdpi",
                    "Android/Icon/Legacy Icon/hdpi", "Android/Icon/Legacy Icon/mdpi", "Android/Icon/Legacy Icon/ldpi",
            } },

            { "Group/App Store/iOS Icons", new string[] {
                    "Promotional/iOS/App Store Icon",
                    "iPhone/Icon/Application Icon/iPhone 60pt@3x", "iPhone/Icon/Application Icon/iPhone 60pt@2x",
                    "iPad/Icon/Application Icon/iPad Pro 83.5pt@2x", "iPad/Icon/Application Icon/iPad 76pt@2x", "iPad/Icon/Application Icon/iPad 76pt@1x",

                    "iPhone/Icon/Spotlight Icon/iPhone 40pt@3x", "iPhone/Icon/Spotlight Icon/iPhone 40pt@2x",
                    "iPad/Icon/Spotlight Icon/iPad 40pt@2x", "iPad/Icon/Spotlight Icon/iPad 40pt@1x",

                    "iPhone/Icon/Settings Icon/iPhone 29pt@3x", "iPhone/Icon/Settings Icon/iPhone 29pt@2x", "iPhone/Icon/Settings Icon/iPhone 29pt@2x",
                    "iPad/Icon/Settings Icon/iPad 29pt@2x",  "iPad/Icon/Settings Icon/iPad 29pt@1x",

                    "iPhone/Icon/Notifications Icon/iPhone 20pt@3x", "iPhone/Icon/Notifications Icon/iPhone 20pt@2x",
                    "iPad/Icon/Notifications Icon/iPad 20pt@2x", "iPad/Icon/Notifications Icon/iPad 20pt@1x",
            } },

            { "Group/App Store/Both Platforms", new string[] { "Android/Samsung Tall", "Android/Samsung Wide", "Android Tablet/7\" or 10\" Tall", "Android Tablet/7\" or 10\" Wide",
                "iPhone/Screenshot/6.5\" Display Tall", "iPhone/Screenshot/6.5\" Display Wide", "iPhone/Screenshot/5.5\" Display Tall", "iPhone/Screenshot/5.5\" Display Wide", "iPad/Screenshot/12.9\" Display Tall",  "iPad/Screenshot/12.9\" Display Wide" } },
            { "Group/App Store/Both Platforms Tall", new string[] { "Android/Samsung Tall", "Android Tablet/7\" or 10\" Tall",
                "iPhone/Screenshot/6.5\" Display Tall", "iPhone/Screenshot/5.5\" Display Tall",  "iPad/Screenshot/12.9\" Display Tall" } },
            { "Group/App Store/Both Platforms Wide", new string[] { "Android/Samsung Wide", "Android Tablet/7\" or 10\" Wide",
                "iPhone/Screenshot/6.5\" Display Wide", "iPhone/Screenshot/5.5\" Display Wide", "iPad/Screenshot/12.9\" Display Wide" } },
            { "Group/App Store/iOS", new string[] { "iPhone/Screenshot/6.5\" Display Tall", "iPhone/Screenshot/6.5\" Display Wide", "iPhone/Screenshot/5.5\" Display Tall", "iPhone/Screenshot/5.5\" Display Wide", "iPad/Screenshot/12.9\" Display Tall", "iPad/Screenshot/12.9\" Display Wide" } },
            { "Group/App Store/iOS Tall", new string[] { "iPhone/Screenshot/6.5\" Display Tall", "iPhone/Screenshot/5.5\" Display Tall", "iPad/Screenshot/12.9\" Display Tall" } },
            { "Group/App Store/iOS Wide", new string[] { "iPhone/Screenshot/6.5\" Display Wide", "iPhone/Screenshot/5.5\" Display Wide", "iPad/Screenshot/12.9\" Display Wide" } },
            { "Group/App Store/Android", new string[] { "Android/Samsung Tall", "Android/Samsung Wide", "Android Tablet/7\" or 10\" Tall", "Android Tablet/7\" or 10\" Wide"  } },
            { "Group/App Store/Android Tall", new string[] { "Android/Samsung Tall", "Android Tablet/7\" or 10\" Tall" } },
            { "Group/App Store/Android Wide", new string[] { "Android/Samsung Wide", "Android Tablet/7\" or 10\" Wide" } },
            { "Group/App Store/Amazon", new string[] { "Amazon/Fire HD 10", "Amazon/Fire HD 8", "Amazon/Fire 7" } },

            { "Group/Promotional/Twitter", new string[] { "Promotional/Twitter/Visible Header", "Promotional/Twitter/Profile Photo", "Promotional/Twitter/Post Photo", "Promotional/Twitter/In-Stream Photo" } },
            { "Group/Promotional/YouTube", new string[] { "Promotional/YouTube/Thumbnail", "Promotional/YouTube/Channel Cover Photo", "Promotional/YouTube/Channel Profile" } },
            { "Group/Promotional/Unity Asset Store", new string[] { "Promotional/Unity Asset Store/Icon", "Promotional/Unity Asset Store/Card Image", "Promotional/Unity Asset Store/Cover Image", "Promotional/Unity Asset Store/Social Media Image", "Promotional/Unity Asset Store/Screenshot" } },

            { "Group/Promotional/Steam", new string[] { "Promotional/Steam/Capsule/Header", "Promotional/Steam/Capsule/Small", "Promotional/Steam/Capsule/Main", "Promotional/Steam/Capsule/Hero",
               "Promotional/Steam/Community/Icon", "Promotional/Steam/Community/Capsule", "Promotional/Steam/Library/Capsule", "Promotional/Steam/Library/Hero", "Promotional/Steam/Library/Logo", "Promotional/Steam/Event/Cover" } },
            { "Group/Promotional/Oculus", new string[] { "Promotional/Oculus/Cover/Landscape", "Promotional/Oculus/Cover/Square", "Promotional/Oculus/Cover/Portrait", "Promotional/Oculus/Cover/Mini",
                    "Promotional/Oculus/Hero", "Promotional/Oculus/Logo", "Promotional/Oculus/Screenshot", "Promotional/Oculus/Trailer Cover",
                    "Promotional/Oculus/Icon/PC/1", "Promotional/Oculus/Icon/PC/2", "Promotional/Oculus/Icon/PC/3", "Promotional/Oculus/Icon/PC/4", "Promotional/Oculus/Icon/PC/5", "Promotional/Oculus/Icon/PC/6", "Promotional/Oculus/Icon/Mobile" } },
            { "Group/Promotional/Itch", new string[] { "Promotional/Itch/Cover", "Promotional/Itch/Screenshot (Any)" } },
            { "Group/Promotional/Kartridge", new string[] { "Promotional/Kartridge/Icon", "Promotional/Kartridge/Background", "Promotional/Kartridge/Screenshot" } },
            { "Group/Promotional/Twitch", new string[] { "Promotional/Twitch/Icon", "Promotional/Twitch/Banner" } },
        };

        public static Dictionary<string, Resolution> All(bool includeAllTypes, bool includeGroups)
        {
            Dictionary<string, Resolution> resolutionDictionary = GameView.AllSizes(true);

            UnityEditor.GameViewSizeGroupType[] typesToInclude = null;
            if (includeAllTypes)
                typesToInclude = new UnityEditor.GameViewSizeGroupType[] { UnityEditor.GameViewSizeGroupType.Android, UnityEditor.GameViewSizeGroupType.iOS };
            else
                typesToInclude = new UnityEditor.GameViewSizeGroupType[] { GameView.GetCurrentGroupType() };

            for (int i = 0; i < typesToInclude.Length; ++i)
            {
                UnityEditor.GameViewSizeGroupType groupType = typesToInclude[i];
                if (AdditionalResolutions.forGroupType.ContainsKey(groupType))
                {
                    Dictionary<string, Resolution> additionalResolutions = AdditionalResolutions.forGroupType[groupType];

                    foreach (string resolutionName in additionalResolutions.Keys)
                    {
                        // If user added sizes from additional resolutions, remove them and use the cleaner multi-layer approach
                        string trimmedSizeName = AdditionalResolutions.ConvertToGameViewSizeName(resolutionName);
                        if (resolutionDictionary.ContainsKey(trimmedSizeName))
                            resolutionDictionary.Remove(trimmedSizeName);

                        if (!resolutionDictionary.ContainsKey(resolutionName))
                            resolutionDictionary[resolutionName] = additionalResolutions[resolutionName];
                    }
                }
            }

            foreach (string resolutionName in AdditionalResolutions.promotional.Keys)
            {
                // If user added sizes from additional resolutions, remove them and use the cleaner multi-layer approach
                string trimmedSizeName = AdditionalResolutions.ConvertToGameViewSizeName(resolutionName);
                if (resolutionDictionary.ContainsKey(trimmedSizeName))
                    resolutionDictionary.Remove(trimmedSizeName);

                if (!resolutionDictionary.ContainsKey(resolutionName))
                    resolutionDictionary[resolutionName] = AdditionalResolutions.promotional[resolutionName];
            }

            if (includeGroups)
            {
                foreach (string resolutionName in AdditionalResolutions.group.Keys)
                    resolutionDictionary[resolutionName] = AdditionalResolutions.group[resolutionName];
            }

            return resolutionDictionary;
        }

#endif

        public static string ConvertToStructuredFolderName(string resolutionName)
        {
            // Intentionally handling the case of "iPhone " or "iPhone/"
            System.StringComparison ord = System.StringComparison.Ordinal;
            if (resolutionName.StartsWith("iPhone", ord))
                resolutionName = "iOS" + System.IO.Path.DirectorySeparatorChar + "iPhone " + resolutionName.Substring("iPhone ".Length);
            else if (resolutionName.StartsWith("iPad", ord))
                resolutionName = "iOS" + System.IO.Path.DirectorySeparatorChar + "iPad " + resolutionName.Substring("iPad ".Length);
            else if (resolutionName.StartsWith("Android Tablet", ord))
                resolutionName = "Android" + System.IO.Path.DirectorySeparatorChar + resolutionName.Substring("Android Tablet ".Length) + " Tablet";
            else if (resolutionName.StartsWith("Android ", ord))
                resolutionName = "Android" + System.IO.Path.DirectorySeparatorChar + resolutionName.Substring("Android ".Length);
            else
            {
                bool isBuiltInAndroidResolution = resolutionName.StartsWith("HVGA", ord) || resolutionName.StartsWith("WVGA", ord) || resolutionName.StartsWith("FWVGA", ord)
                                                                || resolutionName.StartsWith("WSVGA", ord) || resolutionName.StartsWith("WXGA", ord)
                                                                || resolutionName.StartsWith("3:2 Portrait", ord) || resolutionName.StartsWith("3:2 Landscape", ord)
                                                                || resolutionName.StartsWith("16:10 Portrait", ord) || resolutionName.StartsWith("16:10 Landscape", ord);
                if (isBuiltInAndroidResolution)
                    resolutionName = "Android" + System.IO.Path.DirectorySeparatorChar + resolutionName;
            }

            return resolutionName;
        }

        public static string ConvertToGameViewSizeName(string key)
        {
            string[] nameComponents = key.Split('/');

            string name = "";
            if (nameComponents.Length > 1)
                name = nameComponents[nameComponents.Length - 2] + " " + nameComponents[nameComponents.Length - 1];
            else
                name = nameComponents[0];
            return name;
        }
    }
}