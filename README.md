# Turniej Rycerski - Reksio i Wehikuł Czasu

Projekt rekonstrukcji minigry obecnej w grze "Reksio i Wehikuł Czasu", prace są wykonywane w wolnym czasie i wedle uznania.

## Krótki opis oryginalnej minigry

Minigra polegała na sterowaniu rycerzem po mapie w stylu średniowiecznym podzielonej na 3 linie odgrodzone płotkami, podgląd mapy jest widoczny w dolnej części ekranu w formie minimapy.
Rozgrywka zaczynała się z 4 graczami zaczynającymi w rogach mapy i mających na celu strącić rycerzy przeciwników z konii aż do utraty życia.
Były dostępne 3 różne konie do wyboru różniące się statystykami takimi jak prędkość i siła obrażeń.
Oryginalna gra była dostępna tylko w formie singleplayer w jednym momencie fabularnym.

Screen z oryginalnej gry:\
<img src="https://github.com/user-attachments/assets/c2c98ec5-d173-4afc-9979-4a88dc35beac" width="480" height="270">

Przykładową rozgrywkę można zobaczyć na filmie od twórcy fandomowego KKR: [link](https://youtu.be/n0p7G_H1h70?t=4885)

## Build

Link do itch.io skąd można uzyskać ostatnią zbudowaną i działającą wersję projektu: [link](https://merloj.itch.io/turniej-rycerski-riwc)

## Aktualny stan projektu (najważniejsze, wykonane cele)

- Odtworzono najważniejsze elementy rozgrywki
  - W tym: Rycerzy, konie z różnymi statystykami, zachowanie Mapy i Minimapy, możliwość wygrania lub przegrania rozgrywki
  - Wykorzystane zostały oryginalne grafiki z gry Reksio i Wehikuł Czasu wyekstraktowane za pomocą znanego społeczności programu Anndrzem: [link](https://github.com/mysliwy112/AM-transcoder)
- Mozliwość dołączania do rozgrywki lokalnej w formie podłączenia się padami i/lub podziału klawiatury na 2 graczy (do maksymalnie 4 graczy)
  - Aktualny sposób dołączania i sterowania w grze są opisane na stronie na itch.io zanim skończę refaktor tej części projektu (link powyżej w sekcji "Build")

Prezentacja aktualnego stanu projektu w formie gif'ów (niestety drugi gif ulega korupcji przy nagrywaniu z powodu zbyt dużej liczby zmian co klatkę, ale widać, że projekt faktycznie funkcjonuje):\
<img src="https://github.com/user-attachments/assets/495fffb0-6e48-49d7-9ab4-6c9bbac14815" width="319" height="175">
<img src="https://github.com/user-attachments/assets/7596dc78-a841-446d-b94b-d28e7897c5e4" width="319" height="180">

## Aktualne prace i plany na przyszłość

- (Aktualne prace) Refaktor aktualnego systemu dołączania lokalnego celem późniejszej implementacji sieciowej
  - Również po to, aby był bardziej przystępny od tego który jest obecnie (miał on na celu tylko działać, był robiony z myślą o refaktorze)
- Gracze sterowani przez komputer dający możliwość rozgrywki solo jak również, aby dopełnić arenę do 4 graczy, jeżeli jest ich niewystarczająco na kanapie tudzież sofie.
- Implementacja rozgrywki sieciowej pozwalającej połączyć się graczom przez internet i 
- Komentator głosowy reagujący na stan rozgrywki
  - Ostatni, bardzo ważny element humorystyczny z oryginalnej minigry do implementacji
- Tryb turniejowy, w którym do 16 graczy będzie w stanie dołączyć do jednego, dużego turnieju i rozgrywać kolejne pojedynki w ćwierćfinale, półfinale i wielkim finale czwórkami
