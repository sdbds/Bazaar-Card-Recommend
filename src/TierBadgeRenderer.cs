using UnityEngine;

namespace BazaarRecommend
{
    public static class TierBadgeRenderer
    {
        private const float LabelCharacterSize = 0.25f;
        private const int LabelFontSize = 48;

        private static Mesh _circleMesh;

        public static void RemoveBadge(Transform parent, string badgeName)
        {
            var existing = parent.Find(badgeName);
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
                Object.Destroy(existing.gameObject);
            }
        }

        public static void CreateBadge(GameObject card, BoxCollider col, string badgeName, string tier)
        {
            float w   = col.size.x;   // 宽（随卡牌尺寸变化）
            float h   = col.size.z;   // 高 Z 轴
            float ty  = col.size.y;   // 厚 Y 轴
            Vector3 ctr = col.center;

            float badgeSize = 0.28f;

            const float fixedZ = -(2.85f / 2f - 0.18f); // = -1.245，对应 Small 卡底角
            Vector3 pos = new Vector3(
                ctr.x + w / 2f - badgeSize / 2f - 0.04f,
                ctr.y + ty / 2f + 0.005f,
                ctr.z + fixedZ
            );

            CreateBadge(card, badgeName, tier, pos, badgeSize);
        }

        public static void CreateSkillSelectionBadge(GameObject card, BoxCollider col, string badgeName, string tier)
        {
            float w   = col.size.x;
            float h   = col.size.z;
            float ty  = col.size.y;
            Vector3 ctr = col.center;

            float badgeSize = 0.20f;
            float inset = 0.02f;

            Vector3 pos = new Vector3(
                ctr.x + w / 2f - badgeSize / 2f - inset,
                ctr.y + ty / 2f + 0.005f,
                ctr.z - h / 2f + badgeSize / 2f + inset
            );

            CreateBadge(card, badgeName, tier, pos, badgeSize);
        }

        public static bool TryCreateCompactBadge(GameObject card, string badgeName, string tier)
        {
            if (!TryGetLocalBoundsFromColliders(card, out var bounds) &&
                !TryGetLocalBoundsFromRenderers(card, out bounds))
                return false;

            return TryCreateBadgeFromBounds(card, badgeName, tier, bounds, 0.28f, 0.12f, 0.22f, 0.03f);
        }

        public static bool TryCreateSkillSelectionBadge(GameObject card, string badgeName, string tier)
        {
            if (!TryGetLocalBoundsFromColliders(card, out var bounds) &&
                !TryGetLocalBoundsFromRenderers(card, out bounds))
                return false;

            return TryCreateBadgeFromBounds(card, badgeName, tier, bounds, 0.18f, 0.10f, 0.16f, 0.008f);
        }

        private static bool TryCreateBadgeFromBounds(
            GameObject card,
            string badgeName,
            string tier,
            Bounds bounds,
            float sizeRatio,
            float minWidth,
            float maxWidth,
            float insetRatio)
        {
            float baseSize = Mathf.Min(bounds.size.x, bounds.size.z);
            if (baseSize <= 0f)
                baseSize = Mathf.Max(bounds.size.x, bounds.size.z);
            if (baseSize <= 0f)
                return false;

            float badgeSize = Mathf.Clamp(baseSize * sizeRatio, minWidth, maxWidth);
            float inset = Mathf.Max(baseSize * insetRatio, 0.005f);
            float heightOffset = Mathf.Max(bounds.size.y * 0.08f, 0.005f);

            Vector3 pos = new Vector3(
                bounds.max.x - badgeSize / 2f - inset,
                bounds.max.y + heightOffset,
                bounds.min.z + badgeSize / 2f + inset
            );

            CreateBadge(card, badgeName, tier, pos, badgeSize);
            return true;
        }

        private static void CreateBadge(GameObject card, string badgeName, string tier, Vector3 localPosition, float badgeSize)
        {
            Color color = GetTierColor(tier);
            string label = tier.Substring(0, 1); // S / A / B ...

            var bg = new GameObject(badgeName);
            bg.name = badgeName;
            bg.transform.SetParent(card.transform, false);
            bg.transform.localPosition = localPosition;
            bg.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            bg.transform.localScale = new Vector3(badgeSize, badgeSize, 1f);

            bg.AddComponent<MeshFilter>().sharedMesh = GetCircleMesh();
            var mr = bg.AddComponent<MeshRenderer>();
            mr.material = GetMat(color);
            mr.sortingOrder = 100;

            AddTierText(bg.transform, label);
        }

        private static void AddTierText(Transform parent, string label)
        {
            var outlineColor = new Color32(0x5A, 0x2C, 0x16, 0xFF);
            var shadowColor = new Color32(0x20, 0x12, 0x10, 0xD0);
            var faceColor = new Color32(0xFF, 0xF8, 0xDE, 0xFF);

            AddTextMesh(parent, "TierLabelShadow", label, new Vector3(0.022f, -0.022f, 0.08f), shadowColor, 101);

            const float outlineOffset = 0.014f;
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(-outlineOffset, 0f, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(outlineOffset, 0f, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(0f, -outlineOffset, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(0f, outlineOffset, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(-outlineOffset, -outlineOffset, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(-outlineOffset, outlineOffset, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(outlineOffset, -outlineOffset, 0.09f), outlineColor, 102);
            AddTextMesh(parent, "TierLabelOutline", label, new Vector3(outlineOffset, outlineOffset, 0.09f), outlineColor, 102);

            AddTextMesh(parent, "TierLabel", label, new Vector3(0f, 0f, 0.1f), faceColor, 103);
        }

        private static void AddTextMesh(
            Transform parent,
            string name,
            string label,
            Vector3 localPosition,
            Color color,
            int sortingOrder)
        {
            var textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            textGO.transform.localPosition = localPosition;
            textGO.transform.localScale = Vector3.one;

            var tm = textGO.AddComponent<TextMesh>();
            tm.text          = label;
            tm.alignment     = TextAlignment.Center;
            tm.anchor        = TextAnchor.MiddleCenter;
            tm.color         = color;
            tm.fontSize      = LabelFontSize;
            tm.fontStyle     = FontStyle.Bold;
            tm.characterSize = LabelCharacterSize;

            var tmMR = textGO.GetComponent<MeshRenderer>();
            tmMR.sortingOrder = sortingOrder;
        }

        private static Mesh GetCircleMesh()
        {
            if (_circleMesh != null)
                return _circleMesh;

            const int segments = 48;
            var vertices = new Vector3[segments + 1];
            var triangles = new int[segments * 3];
            vertices[0] = Vector3.zero;

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.PI * 2f * i / segments;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * 0.62f, Mathf.Sin(angle) * 0.62f, 0f);
            }

            for (int i = 0; i < segments; i++)
            {
                int next = i == segments - 1 ? 1 : i + 2;
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = next;
            }

            _circleMesh = new Mesh
            {
                name = "BazaarRecommendTierBadgeCircle",
                vertices = vertices,
                triangles = triangles
            };
            _circleMesh.RecalculateNormals();
            _circleMesh.RecalculateBounds();
            return _circleMesh;
        }

        private static bool TryGetLocalBoundsFromColliders(GameObject root, out Bounds localBounds)
        {
            localBounds = new Bounds();
            bool hasBounds = false;
            var colliders = root.GetComponentsInChildren<Collider>(false);
            foreach (var collider in colliders)
            {
                if (collider == null || !collider.enabled)
                    continue;
                EncapsulateWorldBounds(root.transform, collider.bounds, ref localBounds, ref hasBounds);
            }
            return hasBounds;
        }

        private static bool TryGetLocalBoundsFromRenderers(GameObject root, out Bounds localBounds)
        {
            localBounds = new Bounds();
            bool hasBounds = false;
            var renderers = root.GetComponentsInChildren<Renderer>(false);
            foreach (var renderer in renderers)
            {
                if (renderer == null || !renderer.enabled)
                    continue;
                EncapsulateWorldBounds(root.transform, renderer.bounds, ref localBounds, ref hasBounds);
            }
            return hasBounds;
        }

        private static void EncapsulateWorldBounds(Transform root, Bounds worldBounds, ref Bounds localBounds, ref bool hasBounds)
        {
            Vector3 min = worldBounds.min;
            Vector3 max = worldBounds.max;
            EncapsulateLocalPoint(root, new Vector3(min.x, min.y, min.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(min.x, min.y, max.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(min.x, max.y, min.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(min.x, max.y, max.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(max.x, min.y, min.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(max.x, min.y, max.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(max.x, max.y, min.z), ref localBounds, ref hasBounds);
            EncapsulateLocalPoint(root, new Vector3(max.x, max.y, max.z), ref localBounds, ref hasBounds);
        }

        private static void EncapsulateLocalPoint(Transform root, Vector3 worldPoint, ref Bounds localBounds, ref bool hasBounds)
        {
            Vector3 localPoint = root.InverseTransformPoint(worldPoint);
            if (!hasBounds)
            {
                localBounds = new Bounds(localPoint, Vector3.zero);
                hasBounds = true;
            }
            else
            {
                localBounds.Encapsulate(localPoint);
            }
        }

        private static Material GetMat(Color color)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = color;
            return mat;
        }

        private static Color GetTierColor(string tier)
        {
            switch (tier)
            {
                case "S Tier": return new Color32(0x30, 0xA2, 0xFF, 0xFF);
                case "A Tier": return new Color32(0x00, 0xE6, 0x76, 0xFF);
                case "B Tier": return new Color32(0xE7, 0xF5, 0x25, 0xFF);
                case "C Tier": return new Color32(0xFF, 0xA4, 0x4A, 0xFF);
                case "D Tier": return new Color32(0xFF, 0x59, 0x7A, 0xFF);
                case "F Tier": return new Color32(0xE5, 0x7F, 0xF5, 0xFF);
                default:       return new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
        }
    }
}
