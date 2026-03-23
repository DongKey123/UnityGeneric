# Framework Structure

Unity Generic Framework의 프로젝트 구조와 아키텍처를 정의합니다.

---

## 디렉토리 구조

```
Assets/
├── Project/                    # 프로젝트 전용 에셋 (게임 콘텐츠)
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Scenes/
│   └── Resources/
│
└── Scripts/
    └── Framework/              # 제네릭 재사용 프레임워크
        ├── Core/               # 핵심 유틸리티
        │   ├── Singleton/
        │   ├── ObjectPool/
        │   └── EventSystem/
        ├── Patterns/           # 디자인 패턴 구현체
        │   ├── StateMachine/
        │   ├── Observer/
        │   └── Command/
        ├── Extensions/         # C# / Unity 확장 메서드
        ├── Interfaces/         # 공용 인터페이스 정의
        └── Utils/              # 수학, 타이머, 코루틴 헬퍼 등
```

---

## 레이어 아키텍처

```
┌─────────────────────────────────┐
│         Game Layer              │  ← 게임 로직 (Project/)
│  PlayerController, EnemyAI ...  │
├─────────────────────────────────┤
│        System Layer             │  ← 게임 시스템
│  CombatSystem, InventorySystem  │
├─────────────────────────────────┤
│       Framework Layer           │  ← 재사용 프레임워크 (Framework/)
│  Singleton, Pool, StateMachine  │
└─────────────────────────────────┘
```

- **상위 레이어**는 하위 레이어에 의존할 수 있습니다.
- **하위 레이어**는 상위 레이어에 의존하지 않습니다.
- 레이어 간 통신은 인터페이스 또는 이벤트를 통해 이루어집니다.

---

## 네임스페이스 규칙

Framework 코드에만 네임스페이스를 적용합니다.

```csharp
namespace Framework.Core.Singleton { }
namespace Framework.Core.ObjectPool { }
namespace Framework.Core.EventSystem { }
namespace Framework.Patterns.StateMachine { }
namespace Framework.Patterns.Observer { }
namespace Framework.Patterns.Command { }
namespace Framework.Extensions { }
namespace Framework.Interfaces { }
namespace Framework.Utils { }
```

---

## 의존성 규칙

- **Framework 모듈 간 의존**: 최소화, 필요 시 인터페이스로 분리
- **Project → Framework**: 허용
- **Framework → Project**: 금지

---

## 씬 구조

```
Scenes/
├── Boot.unity         # 초기화 전용 씬 (GameManager, 서비스 초기화)
├── Persistent.unity   # 씬 전환 시에도 유지되는 오브젝트
└── Game/
    ├── Stage01.unity
    └── Stage02.unity
```
