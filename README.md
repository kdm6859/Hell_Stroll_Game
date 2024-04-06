## 프로젝트 소개

- 몬스터 배치 및 경로 설정 기능 Custom Editor로 구현
- 플레이어와 몬스터 움직임, 공격 시스템 FSM으로 구현
- 몬스터의 움직임에 추가적으로 AI Navigation 사용

## 시연 영상

https://youtu.be/3MrO6VUGS1Y

## 목차

1. 몬스터 배치 및 경로 설정
2. 몬스터 움직임
3. 플레이어 움직임

### 1. 몬스터 배치 및 경로 설정

- Scene 좌측 상단에 에디터 메뉴얼 출력
- Scene에서 ctrl키를 누른 상태로 마우스 왼쪽 버튼 클릭 시 경로 포인트 추가
- 포인트에 핸들을 생성하여 위치 조정 가능
- A키로 새로운 경로 생성
- C키으로 경로 변경 가능
- shift + delete 로 경로 삭제

![Untitled (1)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/0bf6067f-e49e-437c-b716-2ef027747b6c)

- Play 시 경로의 첫번째 포인트에 몬스터 생성
- 몬스터는 경로를 따라 움직임
- 플레이어 추적 시 경로를 이탈, 추적 포기 시 다시 경로 복귀

![Untitled (2)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/c03ecbb2-7a28-4c47-aacd-21d5e77409ee)

### 2. 몬스터 움직임

- FSM과 AI Navigation을 이용하여 몬스터 움직임 구현
- 몬스터는 지정된 경로를 따라 움직임
- 일정 범위 내에 플레이어 접근 시 플레이어 추적
- 플레이어와 거리가 가까워지면 플레이어를 공격
- 플레이어와의 거리가 멀어지면 추적을 멈추고 경로로 복귀

![Untitled (3)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/971d2115-a683-44da-8d3e-2c87027ddf73)

![Untitled (4)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/de4213f4-7dfc-4143-b8bd-d1a00bdbad1c)

![Untitled (5)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/6f621e6d-8e4b-4652-828c-61a1b56d30cf)

### 3. 플레이어 움직임

- FSM을 이용하여 플레이어움직임 구현
- 전략패턴을 사용하여 공격 모드를 두가지 형태로 구현 (마법, 검)
  
![Untitled (6)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/c28cebfe-74c3-49ef-8991-f2f7964c12af)
![Untitled (7)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/81a8150a-17ca-4fcb-aa64-05a058c4c5bb)
![Untitled (8)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/e335a9b4-1269-460e-8698-ad30c2bb1da3)
![Untitled (9)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/93b938f3-3fcf-4f9c-a497-4ed6234414f8)
![Untitled (10)](https://github.com/kdm6859/Hell_Stroll_Game/assets/64892955/d0d36454-c1b6-439c-9390-4d3c72226514)
