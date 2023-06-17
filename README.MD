## **v1.1.0**

The agents now don't get a negative reward if the opponent throws the ball into its own half. 
Rewards for hitting the ball into the other half are now more bland.

The agents also get bigger negative reward if they throw the ball out of bounds; the opponent doesn't get a negative reward for this.

The agents that scores keeps getting a positive reward, but the other agent now gets a negative reward.

<br>

### **Rewards**: 

- *Agent hits ball*: +0.1
- *Agent throws ball out of bounds (End Episode)*: 
  - Hitter: -0.5
- *Agent throws ball into opponent half*:
  - Hitter: +0.5
- *Agent scores*:
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
  - Other: -1
- *Agent scores*:
  - Hitter: +1


After 50M steps:
The agents now start at 0.01f Y instead of Range(0.5f, 3.75f); 
