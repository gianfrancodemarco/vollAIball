

v1.0.0

Rewards: 

- Agent hits ball: +0.1
- Agent throws ball out of bounds (End Episode): 
  - Hitter: -0.1
  - Other: +0.1
- Agent throws ball into opponent half:
  - Hitter: +1
  - Other: -1
- Agent scores:
  - Hitter: +1


After 50M steps:
The agents now start at 0.01f Y instead of Range(0.5f, 3.75f); 