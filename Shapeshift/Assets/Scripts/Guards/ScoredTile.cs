using UnityEngine;
using System;
using System.Collections;

public class ScoredTile : IComparable<ScoredTile> {

	private Tile tile;
	private float score;

	public Tile EnclosedTile {
		get { return tile; }
	}

	public float Score {
		get { return score; }
	}

	public ScoredTile(Tile tile, float score) {
		this.tile = tile;
		this.score = score;
	}

	public int CompareTo(ScoredTile other) {
		if (score.CompareTo (other.Score) != 0) {
			return score.CompareTo (other.Score);
		}
		if (tile.X.CompareTo(other.EnclosedTile.X) != 0) {
			return tile.X.CompareTo(other.EnclosedTile.X);
		}
		return tile.Y.CompareTo (other.EnclosedTile.Y);
	}

	public bool Equals(ScoredTile other) {
		return (tile.Equals (other.EnclosedTile) && score.Equals (other.Score));
	}

	public override bool Equals (object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != typeof (ScoredTile)) return false;
		return Equals((ScoredTile) obj);
	}

	public override int GetHashCode() {
		return tile.GetHashCode () ^ score.GetHashCode ();
	}
}
