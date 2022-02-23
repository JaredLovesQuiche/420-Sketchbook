ArrayList<Agent> agents = new ArrayList<Agent>();

FlowFieldGrid grid;

void setup()
{
  size(1000, 800);
  
  grid = new FlowFieldGrid();
  colorMode(HSB);
}

void draw()
{
  background(150);
  
  if (mousePressed)
  {
    agents.add(new Agent());
  }
  
  if (Keys.onDown(37))
  {
    grid.res--;
    grid.build();
  }
  if (Keys.onDown(39))
  {
    grid.res++;
    grid.build();
  }
  
  grid.update();
  grid.draw();
  
  fill(255, 255, 255, 50);
  for (Agent a : agents)
  {
    a.update();
    a.draw();
  }
  
  Keys.update();
}
