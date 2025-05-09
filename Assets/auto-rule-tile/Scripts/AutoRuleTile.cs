using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// ----------------------------------------------------------------------------
// Author: Alexandre Brull - Pandaroo
// https://pandaroo.be
// ----------------------------------------------------------------------------

namespace Pandaroo.autoruletile {
	[ExecuteInEditMode]
	[CreateAssetMenu(fileName = "New Auto Rule Tile", menuName = "Tiles/Auto Rule Tile")]
	public class AutoRuleTile : ScriptableObject {
#if(UNITY_EDITOR)

		private RuleTile RuleTileTemplate_Default;

		[SerializeField]
		private Texture2D TileMap;
		[SerializeField]
		private RuleTile RuleTileTemplate;
		[SerializeField]
		private RuleTile OverrideExistingRuleTile;
		[SerializeField]
		private Tile.ColliderType DefaultCollider = Tile.ColliderType.Grid;


		private void OnEnable() {
			OnValidate();
		}

		private void OnValidate() {
			if(RuleTileTemplate_Default != null)
				return;
			// If there is a default template, load it when the asset is created.
			RuleTileTemplate_Default = Resources.Load("AutoRuleTile_default") as RuleTile;
			if(RuleTileTemplate_Default != null) {
				RuleTileTemplate = RuleTileTemplate_Default;
			}
		}

		public void OverrideRuleTile() {
			// Make a copy of the Rule Tile Template from a new asset.
			RuleTile _new = CreateInstance<RuleTile>();
			EditorUtility.CopySerialized(RuleTileTemplate, _new);

			// Get all the sprites in the Texture2D file (TileMap)
			string spriteSheet = AssetDatabase.GetAssetPath(TileMap);
			Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet)
				.OfType<Sprite>().ToArray();

			if(sprites.Length != RuleTileTemplate.m_TilingRules.Count) {
				Debug.LogWarning("The Tilemap doesn't have the same number of sprites than the Rule Tile template has rules.");
			}

			_new.m_DefaultColliderType = DefaultCollider;
			_new.m_DefaultSprite = sprites[24];
			// Set all the sprites of the TileMap.
			for(int i = 0; i < RuleTileTemplate.m_TilingRules.Count; i++) {
				_new.m_TilingRules[i].m_Sprites[0] = sprites[i];
				_new.m_TilingRules[i].m_ColliderType = DefaultCollider;
			}

			// Replace this Asset with the new one.
			if(OverrideExistingRuleTile != null) {
				string name = OverrideExistingRuleTile.name;
				EditorUtility.CopySerialized(_new, OverrideExistingRuleTile);
				OverrideExistingRuleTile.name = name;
				DestroyImmediate(_new);
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
				Selection.activeObject = OverrideExistingRuleTile;
			} else{
				AssetDatabase.CreateAsset(_new, AssetDatabase.GetAssetPath(this));
				Selection.activeObject = _new;
			}
		}


#endif
	}

}

