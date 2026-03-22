using System;
using BazaarGameClient.Domain.Models.Cards;
using BazaarRecommend;
using HarmonyLib;
using UnityEngine;

namespace BazaarRecommend
{
    [HarmonyPatch(typeof(ItemController), "ShowCard")]
    public static class TierIndicatorPatch
    {
        private const string BadgeName = "JulesTierBadge";

        private static Material GetMat(Color color)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = color;
            return mat;
        }

        [HarmonyPostfix]
        static void Postfix(ItemController __instance, bool show)
        {
            try
            {
                // 清除旧 badge
                var existing = __instance.transform.Find(BadgeName);
                if (existing != null) UnityEngine.Object.Destroy(existing.gameObject);

                if (!show) return;

                var itemCard = __instance.CardData as ItemCard;
                if (itemCard == null) return;

                string cardId = itemCard.Template?.Id.ToString();
                string hero = null;
                var heroes = itemCard.Template?.Heroes;
                if (heroes != null && heroes.Count > 0)
                    foreach (var h in heroes) { hero = h.ToString(); break; }
                string tier = CardTierList.GetTier(cardId, hero);
                if (string.IsNullOrEmpty(tier)) return;

                var col = __instance.GetComponent<BoxCollider>();
                if (col == null) return;

                Color color = GetTierColor(tier);
                string label = tier.Substring(0, 1); // S / A / B ...

                CreateBadge(__instance.gameObject, col, color, label);
                Console.WriteLine($"[TierBadge] {itemCard.Template?.InternalName} ({itemCard.Template?.Size}) = {tier}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TierBadge] ERROR: {ex}");
            }
        }

        private static void CreateBadge(GameObject card, BoxCollider col, Color color, string label)
        {
            float w   = col.size.x;   // 宽（随卡牌尺寸变化）
            float h   = col.size.z;   // 高 Z 轴
            float ty  = col.size.y;   // 厚 Y 轴
            Vector3 ctr = col.center;

            // badge 固定大小，位置锚定在右下角
            float badgeW = 0.30f;
            float badgeH = 0.28f;

            // X：各尺寸右边缘内缩
            // Z：固定 local offset（以 Small 卡底角为基准），使所有尺寸 badge 的 world Z 一致
            const float fixedZ = -(2.85f / 2f - 0.18f); // = -1.245，对应 Small 卡底角
            Vector3 pos = new Vector3(
                ctr.x + w / 2f - badgeW / 2f - 0.04f,
                ctr.y + ty / 2f + 0.005f,
                ctr.z + fixedZ
            );

            // 背景 Quad：旋转 90° 使其躺平在 XZ 平面，法线朝 +Y（从上方可见）
            var bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bg.name = BadgeName;
            UnityEngine.Object.Destroy(bg.GetComponent<MeshCollider>());
            bg.transform.SetParent(card.transform, false);
            bg.transform.localPosition = pos;
            bg.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            bg.transform.localScale = new Vector3(badgeW, badgeH, 1f);

            var mr = bg.GetComponent<MeshRenderer>();
            mr.material = GetMat(color);
            mr.sortingOrder = 100;

            // 文字 Label：作为 bg 的子对象，继承同样旋转
            var textGO = new GameObject("TierLabel");
            textGO.transform.SetParent(bg.transform, false);
            // 在 bg 的本地坐标系里，+Z 方向 = 世界 +Y（因为 bg 旋转了 90°），
            // 所以 localPosition.z = 0.1 = 稍微浮在 bg 上方
            textGO.transform.localPosition = new Vector3(0f, 0f, 0.1f);
            // localScale (1,1,1) 会继承 bg 的 (0.30, 0.20, 1) 世界缩放
            textGO.transform.localScale = Vector3.one;

            var tm = textGO.AddComponent<TextMesh>();
            tm.text          = label;
            tm.alignment     = TextAlignment.Center;
            tm.anchor        = TextAnchor.MiddleCenter;
            tm.color         = Color.black;
            tm.fontSize      = 48;
            tm.fontStyle     = FontStyle.Bold;
            tm.characterSize = 0.25f; // localScale 继承 badgeW/badgeH，字符大小自动适配

            var tmMR = textGO.GetComponent<MeshRenderer>();
            tmMR.sortingOrder = 101;
        }

        private static Color GetTierColor(string tier)
        {
            switch (tier)
            {
                case "S Tier": return new Color(1.00f, 0.85f, 0.00f); // 金
                case "A Tier": return new Color(1.00f, 0.55f, 0.00f); // 橙
                case "B Tier": return new Color(0.30f, 0.69f, 0.31f); // 绿
                case "C Tier": return new Color(0.31f, 0.76f, 0.97f); // 蓝
                case "D Tier": return new Color(0.67f, 0.67f, 0.67f); // 灰
                case "F Tier": return Color.white;
                default:       return Color.white;
            }
        }
    }
}
