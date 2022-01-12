ArrayList<PVector> vals = new ArrayList<PVector>();

void setup()
{
  size(500, 600, P2D);
  stroke(255);
  strokeWeight(2);
}

void draw()
{
   background(0);
   
   float time = millis() / 1000.0f;
   
   float valWave = map(sin(time), -1, 1, 0, 1);
   float valRand = random(0, 1);
   float valNoise = noise(time);
   
   // add new vals to list
   vals.add(0, new PVector(0, 0, 0));
   
   // remove last item if too many
   if (vals.size() > width)
     vals.remove(vals.size() - 1);
   
   // draw to screen
   float third = height / 3;
   for (int x = 0; x < vals.size(); x++)
   {
     PVector set = vals.get(x);
     float v1 = set.x;
     float v2 = set.y;
     float v3 = set.z;
     
     
   }
}
