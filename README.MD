## **v2.0.0**

Goal: enforce more volleyball rules
- The same agent should not hit the ball twice in a row
- Hitting the wall is not allowed
- The agents starts from the same position every time, the server in the corner and the receiver in the middle of the court. The ball starts above the server.

## **v1.1.0**

Rewards for hitting the ball into the other half are now more bland.

The agents also get bigger negative reward if they throw the ball out of bounds; the opponent doesn't get a positive reward for this.

<br>

### **Rewards**: 

- *Agent hits ball*: +0.1
- *Agent throws ball out of bounds (End Episode)*: 
  - Hitter: -0.5
- *Agent throws ball into opponent half*:
  - Hitter: +0.5
- *Agent scores (End Episode)*:
  - Hitter: +1
  - Other: -1

<br><br>

## **v1.0.0**

### **Rewards**: 

- *Agent hits ball*: +0.1
- *Agent throws ball out of bounds (End Episode)*: 
  - Hitter: -0.1
  - Other: +0.1
- *Agent throws ball into opponent half*:
  - Hitter: +1
- *Agent scores (End Episode)*:
  - Hitter: +1
  - Other: -1


After 50M steps:
The agents now start at 0.01f Y instead of Range(0.5f, 3.75f); 
