# Unity-Test-Lab

### 기능시험용 서브 프로젝트입니다

이 프로젝트는 게임제작보단 구현에 중점을 둔 상태입니다. 다소 불안하지만 작동여부를 우선으로 제작된 상태이며 또한 인스펙터구조를 통해 값을 조정 가능게 만들었습니다.


### 현재 구현된 기능들:

https://github.com/dasima1125/Unity-Test-Lab/blob/main/Assets/scripts/sc.cs
- 기초적인 이동 기능
   -  좌우 2방향 이동기능
   -  벽 관련 이벤트 구현(벽타기 & 벽점프)
   -  점프 기능(가변점프와 공중점프 기능 구현)
   -  대쉬와 배쉬 기능

https://github.com/dasima1125/Unity-Test-Lab/blob/main/Assets/scripts/scAttack.cs
- 공격 기능
   - 근접공격 기능(애니메이션 연계 3단콤보 구현 및 입력 코루틴을 이용 연속적인 공격 처리)
   - 점프공격 기능(마리오의 찍기 공격과 비슷함)
   - 원거리 공격 기능(ori blind forest 의 공격방식을 참고하여 만듬)

https://github.com/dasima1125/Unity-Test-Lab/blob/main/Assets/scripts/scEnemy.cs
- 적대적 오브젝트 기능
   - 스위치문을 이용해서 모드별 적 ai 조절
   - 모드 0 : 단순추적     , 범위내 플레이어를 추적함
   - 모드 2 : 슬라임       , 범위내 플레이어위치를 저장후 플레이어의 위치로 점프함
   - 모드 3 : 척탄형(기본)  , 범위내 플레이어 감지후 플레이어에게 연속적으로 투사체 투척(프리팹으로 투척물 생성)
   - 모드 4 : 척탄형(산탄)  , 범위내 플레이어 감지후 플레이어에게 3개의 투사체를 한번에 투척(프리팹으로 투척물 생성)
   - 피격시 이벤트 설정 , 간단한 이펙트 추가
     
https://github.com/dasima1125/Unity-Test-Lab/blob/main/Assets/scripts/scThing.cs
- 습득 및 상호작용 오브젝트 기능
   - 모드별 분할 스위치문을 이용 모드값에따라 이벤트 구성 사용.
   - 습득 가능하거나 근처 접근이 자석효과 인스팩터로 지정 가능
   - 문이나 버튼같은 상호작용 설정 (모드 4)
   - 리워드 시스템 추가 오브젝트의 사망판정시 리워드 프리팹 생성

https://github.com/dasima1125/Unity-Test-Lab/blob/main/Assets/scripts/sc%20project/scManager.cs
- 저장 및 매니저 기능
   - 초기 스폰포인트는 각 씬마다 지정 y를 눌러 서브 스폰포인트 지정 가능
   - 사망시 플레이어 오브젝트 비활성 코루틴을 이용 딜레이를 주고 초기상태로 되돌린후 다시 재배치

https://github.com/dasima1125/Unity-Test-Lab/blob/main/Assets/scripts/scNpc_text.cs
- 대화용 npc
   - 간단한 대화 기능 첫 조우와 나머지 상태 분할
     - 2024 12 02 현재 구현 기능 ==> 다층구조를 이용한 대화 내용 저장 , 대화 상호작용 알파버전 구현 (자연스러운 대화 출력 , 대화 스킵 , 이동시 대화 종료)

https://github.com/dasima1125/Unity-Test-Lab/tree/main/Assets/scripts/UI
![Image](https://github.com/user-attachments/assets/0291e513-668c-4883-af99-2c982876a1a4)
- UI 통합 구조
  - UI생성 및 제어,통제 체계화
    - 2025-02-18 구현 기능 ==> 핸들러,옵저버,트리거 > 매니저 > 각 타입별 컨트롤러 순의 객체간 역활분리 및 스트림 구성, 의존성 감소 및 확장성 증대와 동시에 하드코딩을 통한 자원수집을 폴더서칭을 통한 자동초기화
