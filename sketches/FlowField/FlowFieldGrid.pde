class FlowFieldGrid
{
  float[][] data;
  
  int res = 10;
  float zoom = 10;
  
  FlowFieldGrid()
  {
    build();
  }
  
  void build()
  {
    data = new float[res][res];
    
    for (int x = 0; x < data.length; x++)
    {
      for (int y = 0; y < data[x].length; y++)
      {  
        float val = noise(x / zoom, y / zoom);
        val = map(val, 0.4, 0.6, -PI, PI);
        data[x][y] = val;
      }
    }
  }
  
  void update()
  {
    boolean rebuild = false;
    
    if (Keys.onDown(37)) 
    {
      res--;
      rebuild = true;
    }
    if (Keys.onDown(39))
    {
      res++;
      rebuild = true;
    }
    
    if (Keys.onDown(38))
    {
      zoom++;
      rebuild = true;
    }
    if (Keys.onDown(40))
    {
      zoom--;
      rebuild = true;
    }
    
    res = constrain(res, 4, 50);
    zoom = constrain(zoom, 5, 30);
    
    if (rebuild) build();
  }
  
  void draw()
  {
    float w = getCellWidth();
    float h = getCellHeight();
    
    for (int x = 0; x < data.length; x++)
    {
      for (int y = 0; y < data[x].length; y++)
      {  
        float val = data[x][y];
        
        float topleftx = x * w;
        float toplefty = y * h;
        
        pushMatrix();
        translate(topleftx + w / 2, toplefty + h/2);
        
        float hue = map(val, -PI, PI, 0, 255);
        fill(hue, 255, 255);
        noStroke();
        ellipse(0, 0, 20, 20);
        
        rectMode(CENTER);
        noFill();
        stroke(2);
        rect(0, 0, w, h);
        
        rotate(val);
        
        stroke(255);
        line(0, 0, 40, 0);
        popMatrix();
      }
    }
  }
  
  float getCellWidth()
  {
    return width / res;
  }
  
  float getCellHeight()
  {
    return height / res;
  }
  
  float getDirectionAt(PVector p)
  {
    return getDirectionAt(p.x, p.y);
  }
  
  float getDirectionAt(float x, float y)
  {
    int ix = (int)(x / getCellWidth());
    int iy = (int)(y / getCellHeight());
    
   if (ix < 0 || iy < 0 || ix >= data.length || iy >= data[0].length)
   {
     float dy = (height / (float)2) - y;
     float dx = (width / (float)2) - x;
     
     return atan2(dy, dx);
   }
    
    return data[ix][iy];
  }
}
