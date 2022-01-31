

class Dungeon
{
  int roomSize = 10;
  int res = 50;
  int[][] rooms;
  int lilPerBig = 5;
  int lowres() {
    return res / lilPerBig;
  }
  int[][] bigRooms;

  Dungeon()
  {
    generate();
  }

  int getRoom(int x, int y)
  {
    if (x < 0 || y < 0 || x >= rooms.length || y >= rooms.length) return -1;
    return rooms[x][y];
  }

  void setRoom(int x, int y, int t)
  {
    if (x < 0 || y < 0 || x >= rooms.length || y >= rooms.length) return;
    int temp = getRoom(x, y);
    if (temp == 0) rooms[x][y] = t;
  }

  int getBigRoom(int x, int y)
  {
    if (x < 0 || y < 0 || x >= bigRooms.length || y >= bigRooms.length) return -1;
    return bigRooms[x][y];
  }

  void setBigRoom(int x, int y, int t)
  {
    if (x < 0 || y < 0 || x >= bigRooms.length || y >= bigRooms.length) return;
    int temp = getBigRoom(x, y);
    if (temp == 0) rooms[x][y] = t;
  }

  void generate()
  {
    rooms = new int[res][res];

    walkRooms(3, 4);
    walkRooms(2, 2);
    walkRooms(2, 2);
    walkRooms(2, 2);

    makeBigRooms();

    punchHoles();
  }

  void punchHoles()
  {
    for (int x = 0; x < bigRooms.length; x++)
    {
      for (int y = 0; y < bigRooms[x].length; y++)
      {
        int val = getBigRoom(x, y);
        if (val != 1) continue;
        
        if(random(0, 100) < 25) continue;
        
        int[] neighbors = new int[8];
        neighbors[0] = getBigRoom(x, y - 1);
        neighbors[1] = getBigRoom(x + 1, y - 1);
        neighbors[2] = getBigRoom(x + 1, y);
        neighbors[3] = getBigRoom(x + 1, y + 1);
        neighbors[4] = getBigRoom(x, y + 1);
        neighbors[5] = getBigRoom(x - 1, y + 1);
        neighbors[6] = getBigRoom(x - 1, y);
        neighbors[7] = getBigRoom(x - 1, y - 1);
        
        boolean isSolid = neighbors[7] > 0;
        int tally = 0;
        
        for (int n : neighbors)
        {
          boolean s = n > 0;
          
          if (s != isSolid) tally++;
          isSolid = s;
        }
        
        if (tally <= 2)
        {
          setBigRoom(x, y, 0);
        }
      }
    }
  }

  void makeBigRooms()
  {
    int r = lowres();
    bigRooms = new int[r][r];

    for (int x = 0; x < rooms.length; x++)
    {
      for (int y = 0; y < rooms[x].length; y++)
      {
        int val1 = getRoom(x, y);
        int val2 = bigRooms[x / lilPerBig][y / lilPerBig];

        if (val1 > val2)
        {
          bigRooms[x / lilPerBig][y / lilPerBig] = 1;
        }
      }
    }
  }

  void walkRooms(int type1, int type2)
  {
    // walking

    int halfW = rooms.length / 2;
    int halfH = rooms[0].length / 2;

    int x = (int)random(0, rooms.length);
    int y = (int)random(0, rooms[x].length);
    int tx = (int)random(0, halfW);
    int ty = (int)random(0, halfH);

    if (x < halfW) tx += halfW;
    if (y < halfH) ty += halfH;

    setRoom(x, y, type1);
    setRoom(tx, ty, type2);

    while (x != tx || y != ty)
    {
      int dir = (int)random(0, 4);
      int dis = (int)random(1, 4);

      if (random(0, 100) > 50)
      {
        int dx = tx - x;
        int dy = ty - y;

        if (abs(dx) < abs(dy))
        {
          dir = (dy < 0) ? 0 : 1;
        } else
        {
          dir = (dx < 0) ? 3 : 2;
        }
      }

      for (int i = 0; i < dis; i++)
      {
        switch(dir)
        {
        case 0:
          y--;
          break;
        case 1:
          y++;
          break;
        case 2:
          x++;
          break;
        case 3:
          x--;
          break;
        }
        x = constrain(x, 0, res - 1);
        y = constrain(y, 0, res - 1);
        setRoom(x, y, 1);
      }
    }
  }

  void draw()
  {
    noStroke();
    float px = roomSize;
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
          case 4:
            fill(255, 255, 0);
            break;
          default:
            fill(0);
          }
          //rect(x * px, y * px, px, px);
        }
      }
    }
    px = roomSize * lilPerBig;
    for (int x = 0; x < bigRooms.length; x++)
    {
      for (int y = 0; y < bigRooms[x].length; y++)
      {
        int val = bigRooms[x][y];
        if (val > 0)
        {
          switch(val)
          {
          case 1:
            fill(255);
            break;
          case 2:
            fill(255, 0, 0);
            break;
          case 3:
            fill(0, 0, 255);
            break;
          case 4:
            fill(0, 255, 0);
            break;
          default:
            fill(0);
          }
          rect(x * px, y * px, px, px);
        }
      }
    }
  }
}
