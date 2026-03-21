using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BazaarPlusPlus
{
	// Token: 0x0200002A RID: 42
	internal interface IPreviewCardFactory
	{
		// Token: 0x060001B1 RID: 433
		Task<GameObject> CreateCardAsync(PreviewCardSpec spec, Transform parent);

		// Token: 0x060001B2 RID: 434
		Task UpdateCardAsync(GameObject cardObject, PreviewCardSpec spec);

		// Token: 0x060001B3 RID: 435
		void DestroyCard(GameObject cardObject);
	}
}
