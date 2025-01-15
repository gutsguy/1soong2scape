***1soong2scape***


# 개요

---

<aside>
🚨 정신 나간 과학자가 여러분을 원숭이로 바꾸려고 합니다! 동시에 실험체가 된 두 사람은 서로의 힘을 빌려 이곳을 탈출해야 합니다! 🚨
</aside>

이 프로젝트는 **방 탈출 게임**을 기반으로 한 **협동형 퍼즐 게임**입니다. 플레이어는 각 방에 숨겨진 힌트를 서로 공유하며, 주어진 퍼즐을 해결해 다음 방으로 나아가야 합니다. 방마다 놓여져 있는 마이크를 통해 정답을 말해야 합니다. 

# 조원 (이미 원숭이가 되어버린 그들에게 애도를..)
---

|조어진|박재현|
|:------:|:---:|
|![image](https://github.com/user-attachments/assets/f4e035e4-edf4-47d5-8e01-c340f0fbc1e0) | ![image](https://github.com/user-attachments/assets/24245f61-e64d-4fba-a1e2-9c717cd44443) |
|UNIST 20 CSE|DGIST 23|
|Frontend, Backend|Algorithm, Backend|

Special Thanks to - 공진우 : BGM 제공

# 기술 스택

---

|개발 환경|API|서버|언어|
|:---:|:---:|:---:|:---:|
|![image](https://github.com/user-attachments/assets/6d0bca4c-dfd7-401b-8708-759399c893ed) | ![image](https://github.com/user-attachments/assets/41886aa4-7a6e-48e6-b089-5e87d123f033) | ![image](https://github.com/user-attachments/assets/5d32bf21-3c93-4f10-a883-0145ab607b02) | ![image](https://github.com/user-attachments/assets/ae7dc43a-108b-44f5-9ff1-6754ea5b961e) |
| Unity 3D | Google Cloud Speech AI | Photon | C# |



Unity 3D

API
![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/7c272da9-f246-48d5-94c4-7a73848ed09a/image.png)

 Google Cloud
    Speech AI

         서버

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/fe1ee584-cb85-474d-b47c-174a2c69c598/9dc3379f-7d68-4957-9154-bad4ef903a44.png)

       Photon

       언어

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/b59a5376-10fd-4878-8447-0b50816b44c5/image.png)

          C#

# 핵심 기술

---

1. 2인 협동 게임: 두 명의 플레이어에게 서로 다른 힌트가 주어지며, 두 힌트를 조합하여 해당 방의 답을 도출한다.
2. Speech-To-Text (STT): 사용자의 목소리를 받고, Google STT API를 통해 단어를 추출하여 정답 유무를 판단한다.
3. 애니메이션: 숫자 키를 누르면 캐릭터가 각기 다른 애니메이션을 취한다.

# 알고리즘

---

### Huffman Coding

- 사용 빈도에 따라 비트를 적게 사용하도록 하는 Greedy Algorithm이다. 문제를 출제할 때 사용되었다. (”우” And “끼”)

### Levenshtein Distance

- 두 개의 문장을 분석할 때, 필요한 삭제/삽입/변경의 최솟값을 구하여 문장의 유사도를 판단하는 방법. Question 1, 2, 3에 사용되었다.

### Longest Common Substring(LCS)

- 두 문장을 분석할 때, 가장 긴 공통 부분 수열을 찾는 알고리즘. Question 1, 2, 3에 쓰였다.

### Histogram Analysis

- 연속된 데이터를 히스토그램으로 나타내어 면적의 비교를 통해 유사도를 계산하는 방법. Question 4에 사용되었다.

~~Dynamic Time Warping(DTW): Question 4에 쓰일 예정이었다. 코드 구현까지 다 했지만 정확도가 잘 나오지 않아 폐기되었다.~~

~~Mel-Frequency Cepstral Coefficient(MFCC): Question 4에 쓰일 예정이었다. 마찬가지로 코드 구현을 완료했지만 효과가 없어 폐기되었다.~~

# 게임 설명 (스포일러 주의)

## 기본 규칙

- 마이크를 통해 정답을 외치는 방법 (Quesiton 1, 2, 3)
    1. 마이크를 터치한다.
    2. 말을 한다.
    3. 마이크를 다시 터치한다.
    4. 한글이 뜰 때까지 기다린다.
- 마이크를 통해 정답 음성을 들려주는 방법 (Question 4)
    1. 마이크를 터치한다.
    2. 음성을 말한다. (상단 바가 초록색으로 모두 차면 녹음이 자동으로 종료된다.)
    3. 마이크를 다시 터치한다.
    4. 음성이 일치한다면 문이 열린다.
    5. 문이 열리지 않는다면 1~4를 반복한다.
- 숫자 키를 눌러 애니메이션을 볼 수 있다.

## Tutorial

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/228de4d6-c100-4886-8a9c-6f13ca5e5a5b/image.png)

벽에 있는 글이 조작 방법을 알려준다.

첫 방에서는 서로의 모습을 확인할 수 있다.

마이크에 “안녕하세요”를 말하면 다음 방으로 넘어갈 수 있다.

## Question 1

벽에 색깔을 이용한 문제가 있고, 다른 쪽에는 답을 말할 수 있는 마이크가 구비되어 있다.

한 쪽에서 색에 대한 문제를 풀면, 해당 답을 상대에게 전달해주어야 하는 문제이다.

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/5b3caee4-e5c6-4876-8180-f35a8366f48d/image.png)

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/f86cae59-261f-4d6f-94fb-9e639571d35e/image.png)

## Question 2

한 쪽에는 풀어야 하는 수식이 ‘우끼어’로 적혀져 있고, 다른 한 쪽에는 ‘우끼어’를 숫자로 번역하는 방법이 나와 있다. 해당 수식의 답을 숫자로 말하면 다음 방으로 넘어갈 수 있다.

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/db3a46d1-0a73-465c-bde4-100c271887c6/image.png)

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/253ffee4-8d72-4073-a79d-26bf52577744/image.png)

## Question 3

한 쪽에는 ‘우끼어’로 적힌 하나의 단어가 적혀져 있다. 다른 쪽에는 ‘우끼어’를 한글로 번역할 수 있는 키가 주어진다. 이를 통해 우끼어를 번역하여 한글 단어를 말하면 다음 방으로 넘어갈 수 있다.

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/5c74e4ca-9c07-4ede-91df-d2f9e9da177a/image.png)

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/257b2a41-5d87-41d5-800d-fdcbeb930476/image.png)

## Question 4

한 쪽에는 오디오가, 한 쪽에는 마이크가 놓여져 있다. 오디오를 통해 흘러나오는 소리를 상대방에게 알려주면, 마이크를 통해 해당 소리를 그대로 말해야 한다.

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/ccc65ca5-9675-4318-9048-f09336215b35/image.png)

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/e8321ee3-821d-4f63-8eef-4f860b9f6bf0/image.png)

## Clear

원숭이를 위한 해변이 나타난다. 브금이 귀여운 소리로 바뀐다.

![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/9be822bc-fefd-4f33-9839-599f35fbd289/image.png)

# Challenging moment

---

1. 음성을 기반으로 하다 보니 게임 테스트 과정에서 어려움이 있었다. 사람들이 없는 장소를 찾아다녀야 했다.
2. 서버와 쉽게 연결했지만, 서버와 소통하는 것이 어려웠다. 한 쪽에서 애니메이션을 쓰면 모든 캐릭터에게 애니메이션이 적용되었고, 심지어 다른 쪽에서는 보이지 않았다. 문을 동시에 여는 것도 힘들었다. → 하루 전으로 코드를 돌리기도 했다. 닥터 스트레인지 뺨치는 역행 능력
3. 알고리즘을 하나하나 공부해야 하는 부분이 힘들었고, 분석하는 코드를 만들어도 물체 간의 상호작용을 고려하면서 코드를 적용시켜야 한다는 부분이 도전이었다.
4. 많은 고통을 이겨내고 이루었으니 된 거 아닌가?

# 느낀점

---

- 조어진

팀원이 너무 밉다. 좀 잘해줬으면 좋았을텐데

- 박재현

수업 시간에 배운 알고리즘들을 적용시킬 수 있어서 보람있었다. 새로운 알고리즘들을 더 배워가는데, 다음에 어디에 쓰일 지 기대가 된다. 역시 게임을 만드는 것이 과정 자체도 재미있다. 맵을 열심히 꾸며준 어진이 형에게 무한한 감사를 🐵👍

# 응용 프로그램

---

[1soong2scapeReal.zip](https://prod-files-secure.s3.us-west-2.amazonaws.com/f6cb388f-3934-47d6-9928-26d2e10eb0fc/e7924042-74d3-41e2-91e9-ca8e589f75f4/1soong2scapeReal.zip)
