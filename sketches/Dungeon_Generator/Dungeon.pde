

class Dungeon
{
  int roomSize = 10;
  int res = 50;
  int[][] rooms;
  
  Dungeon()
  {
    generate();
  }
  
  void generate()
  {
    rooms = new int[res][res];
  }
  
  void draw()
  {
    noStroke();
    for (int x = 0; x < res; x++)
    {
      for (int y = 0; y < res; y++)
      {
        int val = rooms[x][y];
        if (val > 0)
        {
          switch(val)
          {
            case 1:
              fill(0, 255, 0);
              break;
            case 2:
              fill(255, 0, 0);
              break;
            case 3:
              fill(0, 0, 255);
              break;
          }
          rect(x * roomSize, y * roomSize, roomSize, roomSize);
        }
      }
    }
  }
}
