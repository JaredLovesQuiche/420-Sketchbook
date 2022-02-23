

class Agent
{
  PVector position = new PVector();
  PVector velocity = new PVector();
  PVector force = new PVector();
  float mass = 1;
  float maxSpeed = 10;
  float maxForce = 10;
  
  PVector target = new PVector();
  float targetAngle = 0;
  float targetRadius = 100;
  float targetSpeed = 0;
  
  Agent()
  {
    position = new PVector(mouseX, mouseY);
    velocity = new PVector(random(-5, 5), random(-5, 5));
    mass = random(50, 100);
    maxForce = random(5, 15);
    targetAngle = random(-PI, PI);
    targetRadius = random(50, 100);
    maxSpeed = random(2, 15);
    targetSpeed = map(maxForce, 5, 15, 0.005, 0.05);
  }
  
  void update()
  { 
    PVector offset;
    
    target = PVector.add(position, offset);
    
    doSteeringForce();
    doEuler();
  }
  
  void doSteeringForce()
  {
    // find desired vel
    // desired velocity = clamp(target position - current position)
    
    PVector desiredVelocity = PVector.sub(target, position);
    desiredVelocity.setMag(maxSpeed);
    
    // find steering force
    // steering force is = to desired velocity - current vel
    PVector steeringForce = PVector.sub(desiredVelocity, velocity);
    steeringForce.limit(maxForce);
    
    force.add(steeringForce);
  }
  
  void doEuler()
  {
    // euler integration
    PVector acc = PVector.div(force, mass);
    velocity.add(acc);
    position.add(velocity);
    force.mult(0);
  }
  
  void draw()
  {
    //ellipse(target.x, target.y, 10, 10);
    
    float a = velocity.heading();
    pushMatrix();
    translate(position.x, position.y);
    rotate(a);
    triangle(5, 0, -10, 5, -10, -5);
    popMatrix();
  }
}
