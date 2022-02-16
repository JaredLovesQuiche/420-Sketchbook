

class Agent
{
  color colour;
  
  PVector pos = new PVector();
  PVector vel = new PVector();
  PVector force = new PVector();
  
  float size;
  float mass;
  
  boolean doneCalcingGravity = false;
  
  Agent(float massMin, float massMax)
  {
    pos.x = random(0, width);
    pos.y = random(0, height);
    
    mass = random(massMin, massMax);
    size = sqrt(sqrt(mass));
    
    colorMode(HSB);
    colour = color(random(0, 255), 255, 255);
  }
  
  void update()
  {
    for (Agent a : agents)
    {
      if (a == this) continue;
      if (a.doneCalcingGravity) continue;
      force.add(findGravityForce(a));
    }
    doneCalcingGravity = true;
    
    // a = f/m
    PVector acc = PVector.div(force, mass);
    
    // clear force
    force.set(0, 0);
    
    // v += a
    vel.add(acc);
    
    // p += v
    pos.add(vel);
  }
  
  void draw()
  {
    noStroke();
    fill(colour);
    ellipse(pos.x, pos.y, size, size);
  }
  
  PVector findGravityForce(Agent a)
  {
    PVector vToAgentA = PVector.sub(a.pos, pos);
    
    float r = PVector.sub(a.pos, pos).mag();
    
    float gravForce = G * (a.mass * mass) / (r * r);
    
    if (gravForce > maxForce) gravForce = maxForce;
    
    vToAgentA.normalize();
    vToAgentA.mult(gravForce);
    
    // add force to other object
    a.force.add(PVector.mult(vToAgentA, -1));
    
    return vToAgentA;
  }
}
