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
    - 2025-02-24 구조변경 ![Image](https://github.com/user-attachments/assets/6405519a-2074-4cc5-abc6-f5a2b0d81d8d)
      컴포저를 통한 명령조합방식으로 교체 또한 매니저에 UI관련 정보들 중앙집중화, chatbox기능과 alarm기능을 객체화함으로 컨트롤러 확장 예정

https://github.com/dasima1125/Unity-Test-Lab/tree/main/Assets/scripts/InventoryManager/new%20System
- 인벤토리 시스템 구현

https://github.com/dasima1125/Unity-Test-Lab/tree/main/Assets/scripts/EquipmentManager/new%20system
- 장비 시스템 구현

https://github.com/dasima1125/Unity-Test-Lab/tree/main/Assets/scripts/NotificationSystem
- 각 모듈간 통신 및 중계 시스템

  이 시스템은 Notification 객체를 매개체로 하여 클래스 간 이벤트를 전달하고 중계하는 역할을 합니다.
  핵심 컴포넌트인 NotificationCore는 키(key)와 이벤트 이름(eventName)을 기준으로 델리게이트(리스너)를 등록하고 관리하며,
  필터 기능을 추가해 예외 상황을 최대한 배제하고 안전하고 신뢰성 있는 알림 수신을 지원합니다.
  특정 이벤트 발생 시 해당 델리게이트를 호출하여 연결된 메서드를 실행하며 이 덕분에 서로 직접 참조하지 않는 클래스들 간에도 느슨한 결합으로 통신할 수 있습니다.
  
   #  주의 사항
     이 시스템은 버퍼 시스템을 사용할경우 버퍼 시스템을 처리 타이밍을 내부에서 처리못합니다 예를들자면 게임매니저나 MonoBehaviour 를 상속받은 싱글턴 객체에서
     업데이트 함수로 ProcessBuffer()를 실행시켜주기 바랍니다. 차후 보완 예정입니다.
     버퍼 시스템을 사용안하실꺼면 호출시 처리속도 파라미터를 사용안하시면 됩니다.

   #  주 구조
   - NotificationSystem: 노티피케이션 시스템 초기화와 전체 관리 역할 담당

   - NotificationPort: 외부에서 구독, 해제, 알림 전송을 수행하는 인터페이스 역할

   - NotificationCenter: 구독자 관리와 버퍼(우선순위 포함) 처리, 알림 중계 담당

   - NotificationCore: 구독자 등록/해제 및 알림 호출 핵심 로직 담당

   - Notification: 알림 메시지의 데이터 구조 (키, 발신자, 페이로드 포함)



   #  주요 메서드

  - Send(명령을 요청할 클래스,호출할 함수 ,호출한 클래스)
    
    명령을 해당 클래스에 호출 요청합니다.  
    이 호출은 NotificationSystem에 구독된 이벤트를 실행하며, 호출 형태가 다르거나 이벤트가 없으면 무시됩니다.

    예시:
    ```csharp
    Send("InventorySystem", "UpdateAllSlot", this);
    ```

    또는 
    send(명령을 요청할 클래스, 호출할 함수 ,호출한 클래스 ,호출한 함수에 넣을 매기변수 , 처리우선순위)
    
    이는 처리우선 속도를 사용할때입니다 매개가 없을땐 null 선언을 하시고 그다음에 버퍼스피드를 사용하시면됩니다.

 
    매개가 없을 경우

    ```csharp
    Send("InventorySystem","UpdateAllSlot",this,null,BufferSpeed.Fast);
    ```
    매개가 있을 경우

    ```csharp
    Send("EquipmentSystem","HandleEquipmentAcquisition",this,NewItemSystem_ID,BufferSpeed.Fast)
    ```

    
      

   
  - Subscribe(구독을 신청한 클래스, 구독할 이벤트)
    
    이 메서드는 NotificationSystem에 특정 클래스의 이벤트 핸들러를 등록하여, 해당 이벤트가 발생할 때 지정한 함수가 호출되도록 설정하는 역할을 합니다.
    매개를 전송할 변수를 따로 지정할 필요는 없습니다.

    예시:
    ```csharp
    SubscribePayload("InventorySystem", UpdateAllSlot);
    ```
  - Unsubscribe(취소를 신청할 클래스, 취소할 이벤트)
    
    구독한 이벤트를 NotificationSystem에서 제거합니다.
    NotificationSystem 내 구독하지 않는 이벤트를 구독 시도시 명령은 무시됩니다.

    예시:
    
    ```csharp
    UnsubscribePayload("InventorySystem", UpdateAllSlot)
    ```
  #  처리 흐름

    - Send
      
      ![Image](https://github.com/user-attachments/assets/76a5e706-89a0-4b3d-9426-b7f48a254cf8)

    - Subscribe
   
      ![Image](https://github.com/user-attachments/assets/e14061c3-59cf-4049-83ef-dd27d4cfa174)

    - Unsubscribe
   
      ![Image](https://github.com/user-attachments/assets/61b0b38c-cfbd-4172-86ff-9f9712458dcf)

    # 추가 기능

    - LogSubscription(string key,  bool isSubscribed)
      
      구독여부를 디버그하는 메서드입니다. 해당 시스템의 구독 시작점에 사용되거나, 해지할때 사용됩니다

      예시:
      
      ```csharp
      public void Subscribe(string key, string eventName, Action<Notification> listener)
      {   
        _center.AddListener(key, eventName, listener);
        _debugLog?.Invoke(key, true);
      }
      ```

    - DebugLog(string key, Notification notification , bool success)
 
      호출 성공여부를 나타냅니다. 해당 이벤트가 구독되어 명령이 전달되었는지 확인하며 명령을 해당클래스에 보내기 직전에 사용됩니다.
      하지만 구조적으로 명령을 보내는 걸 확인한거지 해당호출이 타입에 안맞았을경우 무시될경우를 알수는 없습니다(이는 확장클래스가 비대해지는걸 막기위해 한 조치입니다..)

      ```csharp
      public void Notify(string key, string eventName, Notification notification)
      {
        bool logCheck = false;

        if (_listeners.TryGetValue(key, out var eventDict) && eventDict.TryGetValue(eventName, out var handler))
        {
            handler?.Invoke(notification);
            logCheck = true;
        }

        _debugLog?.Invoke(key, notification, logCheck);

      }
      ```

  # 차후 추가예정 기능
   - 개선된 버퍼 처리기능 및 분할처리 기능

      
      
      
      

