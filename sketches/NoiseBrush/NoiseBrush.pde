void setup()
{
  size(600, 300, P2D);
}

void draw()
{
  float time = millis() / 1000.0f;
  float d3 = map(noise(time), 0, 1, 10, 20);

  ellipse(mouseX, mouseY, d3, d3);
}
