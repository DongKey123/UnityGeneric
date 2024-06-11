유니티에서 모바일 Generic Code 


[UIManager]
 UIManager 는 UIBaseController를 통해 UI를 관리
 Canvas 및 RayCaster 를 Cashing하여 렌더링 부하를 감소하여 최적화
 Enable을 껏다 켰다 할 때마다 SetDirty함수를 최소화 하여 관리하기 위해 제작

![UIManager](DrawIO/export/UIManager.drawio.png)

 UIBaseController 기능
  - Show
  - Hide
  - SetSortingOrder
  - NotchArea // Todo: 추후 모바일에서 Notch영역을 적용하기 위해 기능 추가 제작
