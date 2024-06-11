1. GetComponent(), Find() 메소드 사용 줄이기
2. GetComponet 대신 TryGetComponet 사용 
https://medium.com/chenjd-xyz/unity-tip-use-trygetcomponent-instead-of-getcomponent-to-avoid-memory-allocation-in-the-editor-fe0c3121daf6

2019.2 버전부터 나왔으며 GC 걱정 없이 깔끔하게 사용 할 수 있음 

3. Object.name, GameObject.tag 사용하지 않기
CompareTag 같은 함수를 이용해서 사용

4. 비어있는 유니티 이벤트 메소드 방치하지 않기
사용하지 않는 Update 및 Start 함수 지우기

5. StartCoroutine() 자주 호출하지 않기
6. 코루틴의 yield 캐싱하기
7. 메소드 호출 줄이기
 - 가독성이 떨어 질 수 있으므로 함수내의 값이 동일하다면 캐싱하여 사용하자...

8. 참조 캐싱하기

9. 빌드 이후 Debug.Log() 사용하지 않기
 - Todo: Log,Warning,Error 등 래핑하여서 Developer 빌드에서만 사용하도록 수정

10. Transform 변경은 한번에
 - position과 roation을 모두 변경해야 하는 경우에 SetPositionRotation함수 적용하자...

11. 불필요하게 부모 자식 구조 늘리지 않기
12. ScriptableObject 활용하기
 - 공용자산(변하지 않는 것)들은 묶어서 공용 데이터 필드로 사용하자...


13. 필요하지 않은 경우, 리턴하지 않기
14. new로 생성하는 부분 최대한 줄이기
15. 오브젝트 풀링 사용하기
16. 구조체 사용하기
 - 생성/해제가 자주 일어나면 구조체로 만든다.
 - 생성/해제보다 전달(매개변수, 리턴)이 훨씬 자주 일어나면 클래스로 만들거나 매개변수 한정자 in을 사용한다.

17. 컬렉션 재사용하기
18. List 사용할 때 주의할 점
19. StringBuilder 사용하기
20. LINQ 사용 시 주의하기
21. 박싱, 언박싱 피하기
23. Enum HasFlag() 박싱 이슈
24. 비싼 수학 계산 피하기
 - 나눗셈보다는 곱셈으로 처리하기
 - System.Math.Abs vs UnityEngine.Mathf.Abs vs 삼항 연산자 ((x >= 0) ? x : -x) 가 더 빠르니 보이지 않는 코드에선 사용해보자
25. Camera.main
 - 2020.2 버전부터 개선
26. 벡터 연산 시 주의사항
 - vector * (scalar * scalar) 스칼라 값부터 계산하자

 //Todo: Unity 내부 기능이 C++로 제작되어 있어 사용이 많은 것들은 직접 제작하여(Ex Find함수) 최적화 하기

출처: https://rito15.github.io/posts/unity-opt-script-optimization/#objectname-gameobjecttag-%EC%82%AC%EC%9A%A9%ED%95%98%EC%A7%80-%EC%95%8A%EA%B8%B0
