using System;
using System.Collections.Generic;
using System.Linq;

public static class GameChecker
{
    public static bool IsBoardValid(List<Tile> tiles)
    {
        return false;
    }

    private class TileGroup
    {
        public TileGroup(TileValue t)
        {
            _tiles = new List<TileValue>();
            _tiles.Add(t);
            _type = GroupType.Unknown;
        }

        public bool Add(TileValue? tBefore, TileValue t)
        {
            int pos = tBefore == null ? 0 : _tiles.FindIndex(x => x.IsSame(tBefore.Value)) + 1;
            if (!CanAdd(t, pos))
                return false;
            if (tBefore == null)
                _tiles.Insert(0, t);
            else if (pos == _tiles.Count - 1)
                _tiles.Add(t);
            else
                _tiles.Insert(pos, t);
            return true;
        }

        private bool CanAdd(TileValue t, int pos)
        {
            if (_type == GroupType.Group)
                return CanAddGroup(t);
            if (_type == GroupType.Sequence)
                return CanAddSequence(t, pos);
            if (CanAddGroup(t))
            {
                _type = GroupType.Group;
                return true;
            }
            if (CanAddSequence(t, pos))
            {
                _type = GroupType.Sequence;
                return true;
            }
            return false;
        }

        private bool CanAddGroup(TileValue t)
        {
            if (_tiles.Count == 4)
                return false;
            if (t.value == 0)
                return true;
            if (!IsValueValid(t.value) || Any(x => x.color == t.color))
                return false;
            return true;
        }

        private bool CanAddSequence(TileValue t, int pos)
        {
            if (_tiles.Count == 13)
                return false;
            if (t.value == 0)
            {
                if (pos == 0 && _tiles[0].value == 1)
                    return false;
                if (pos == _tiles.Count && _tiles.Last().value == 13)
                    return false;
                return true;
            }
            if (!IsColorValid(t.color))
                return false;
            if (pos > 0 && _tiles[pos - 1].value != t.value - 1)
                return false;
            if (pos < _tiles.Count && _tiles[pos].value != t.value + 1)
                return false;
            return true;
        }

        // Redefine Any from Linq, ignoring Joker
        private bool Any(Func<TileValue, bool> predicate)
        {
            foreach (TileValue t in _tiles)
            {
                if (t.value == 0)
                    continue;
                if (predicate(t))
                    return true;
            }
            return false;
        }

        // Check the color of the list
        // We check the color of the first element, except if it's a joker
        private bool IsColorValid(TileValue.TileColor color)
        {
            foreach (TileValue t in _tiles)
            {
                if (t.value == 0)
                    continue;
                return t.color == color;
            }
            return true;
        }

        private bool IsValueValid(int value)
        {
            foreach (TileValue t in _tiles)
            {
                if (t.value == 0)
                    continue;
                return t.value == value;
            }
            return true;
        }

        private List<TileValue> _tiles;
        private GroupType _type;

        private enum GroupType
        {
            Unknown,
            Sequence,
            Group
        }
    }
}
