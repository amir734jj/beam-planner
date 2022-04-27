---
marp: true
paginate: true
---

## Amir Hesamian
 April 29th, 2022

---

### Math Review

To find angle between vectors:

- The Law of Cosines
$$A \cdot B = |A| \times |B| \times cos(\theta)$$

- Normalize the vectors
$$ \hat{A} \cdot \hat{B} = cos(\theta) $$

- Calculate Arccosine
$$ \theta = cos^{-1}(\hat{A} \cdot \hat{B}) $$

- Convert radian to angle

$$\theta = \frac{\theta^c * 180.0}{\Pi}$$

---

## Parameters

- Each sattelite can serve up to 32 users simultaneously
- Each beam is assigned to one of 4 colors 


---

## Constraints

1) No beam of the same color may be within 10 degrees of each other
2) No beam within 20 degrees of non-startlink satellite
3) From user's perspective the angle of the beam must be within 45 degrees

---

### Constaint #1

*No beam of the same color may be within 10 degrees of each other*

```
for all starlink satellites
  for any two users of a starlink satellite
    assert angle between two vectors:
        satellite->user1
        satellite->user2
      is greater than 10 degrees
```

---

### Constaint #2

*No beam within 20 degrees of non-startlink satellite*

```
for all starlink satellites
  for all users of a starlink satellite
    for all interferes
      assert angle between two vectors:
          satellite->user
          interfere->user
        is greater than 20 degrees
```

---

### Constaint #3

*From user's perspective the angle of the beam must be within 45 degrees*

```
for all starlink satellites
  for all users of a starlink satellite
    assert angle between two vectors:
        origin->user
        origin->satellite
      is greater than 180 - 45 = 135 degrees
```

---

## Solution

- Greedy programming using:
  - C# + ANTLR (parser)
- Demo using:
  - Three.js + React

See the result: https://threejs-earth-satellites.vercel.app

---

### Benchmarking

```shell
Testing 00_example (1 seconds)
Testing 01_simplest_possible (1 seconds)
Testing 02_two_users (1 seconds)
Testing 03_five_users (1 seconds)
Testing 04_one_interferer (1 seconds)
Testing 05_equatorial_plane (1 seconds)
Testing 06_partially_fullfillable (1 seconds)
Testing 07_eighteen_planes (1 seconds)
Testing 08_eighteen_planes_northern (2 seconds)
Testing 09_ten_thousand_users (2 seconds)
Testing 10_ten_thousand_users_geo_belt (2 seconds)
Testing 11_one_hundred_thousand_users (15 seconds)
```
