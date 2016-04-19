using UnityEngine;
using System.Collections;

public class Tile {

	private int xPos;
	private int yPos;

	public int X {
		get { return xPos; }
	}

	public int Y {
		get { return yPos; }
	}

    public Tile(TileItem item) : this(item.tileX, item.tileY)
    {

    }

	public Tile(int xPos, int yPos) {
		this.xPos = xPos;
		this.yPos = yPos;
	}

	public bool Equals(Tile other) {
		return (xPos.Equals (other.X) && yPos.Equals (other.Y));
	}

	public override bool Equals (object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != typeof (Tile)) return false;
		return Equals((Tile) obj);
	}

	public override int GetHashCode() {
		return xPos.GetHashCode () ^ yPos.GetHashCode ();
	}

  public override string ToString() {
    return "Tile{X=" + xPos + ", Y=" + yPos + "}";
  }
}
