using System;
using System.Collections.Generic;
using Hexen.BoardSystem;
using Hexen.HexenSystem;
using Hexen.HexenSystem.PlayableCards;
using Hexen.ReplaySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hexen.GameSystem.Cards
{
    public class BombCard : MonoBehaviour, ICard<HexTile>
    {
        #region Properties
        public Board<Capsule<HexTile>, HexTile> Board { get; set; }
        public Grid<HexTile> Grid { get; set; }
        public PlayableCardName Type { get; set; }
        #endregion

        #region Fields

        [SerializeField] private Image _image;
        [SerializeField] private Text _title;
        [SerializeField] private Text _description;

        #endregion

        void Start()
        {
            _title.text = PlayableCardName.Bomb.ToString();
            _description.text = "Destroys a tile and all it's neighbors, including ALL the enemies and the hero standing on it";
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public bool CanExecute(HexTile atPosition)
        {
            return Positions(atPosition).Contains(atPosition);
        }
        public void Execute(HexTile atPosition)
        {

            foreach (var hexTile in Positions(atPosition))
            {
                Grid.TryGetCoordinateAt(hexTile, out var hexCoordinate);

                if (Grid.HexPositions.ContainsKey(hexTile)) //if there is a tile
                {
                    if (Board.TryGetCapsule(hexTile, out var capsuleOnTile)) //and if there is a capsule on it
                    {
                        Board.Hit(capsuleOnTile);
                        capsuleOnTile.HitFrom(hexTile);
                    }

                    Grid.TryGetCoordinateAt(hexTile, out var coordinate );
                    Grid.Remove(hexTile);
                    hexTile.Hide();
                }
            }
            

        }
        private static int Mod(int x, int m) => (x % m + m) % m;
        public List<HexTile> Positions(HexTile hoveredTile)
        {
            List<HexTile> completeList = new List<HexTile>();
            completeList.Add(hoveredTile);

            for (int i = 0; i < MovementHelper<HexTile>.Offsets.Count; i++)
            {
                var list = new MovementHelper<HexTile>(Board, Grid).CollectAtPosition(hoveredTile, MovementHelper<HexTile>.Offsets[i].x,
                    MovementHelper<HexTile>.Offsets[i].y, 1).CollectValidPositions();

                if (list.Contains(hoveredTile))
                {
                    completeList.Clear();

                    completeList.AddRange(new MovementHelper<HexTile>(Board, Grid).CollectAtPosition(hoveredTile, MovementHelper<HexTile>.Offsets[Mod((i - 1), 6)].x,
                        MovementHelper<HexTile>.Offsets[Mod((i - 1), 6)].y, 1).CollectValidPositions());
                    completeList.AddRange(list);
                    completeList.AddRange(new MovementHelper<HexTile>(Board, Grid).CollectAtPosition(hoveredTile, MovementHelper<HexTile>.Offsets[Mod((i + 1), 6)].x,
                        MovementHelper<HexTile>.Offsets[Mod((i + 1), 6)].y, 1).CollectValidPositions());

                    return completeList;
                }
                else
                {
                    completeList.AddRange(list);
                }
            }
            return completeList;
        }
        public void ResetCard() => gameObject.GetComponent<CardBase>().ResetCard();
        public void Fade() => gameObject.GetComponent<CardBase>().Fade();
        
    }
}