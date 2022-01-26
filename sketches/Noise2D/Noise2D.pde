void setup()
{
  size(400, 400, P2D);
  noStroke();
  colorMode(HSB);
}

void draw()
{
  float zoom = 100;
  float time = millis() / 1000.0f;
  float threshold = mouseX / (float)width;
  
  for (int x = 0; x < width; x++)
  {
    for (int y = 0; y < height; y++)
    {
      float val = map(noise(x/zoom, y/zoom, time), .2, .8, 0, 1);
      
      fill(val < threshold ? 0 : 255);
      rect(x, y, 1, 1);
    }
  }
}
