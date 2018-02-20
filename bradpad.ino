#include <Keyboard.h>

int prevState5 = 0;
int prevState6 = 0;
int prevState7 = 0;

void setup() {
  Serial.begin(9600);
  Keyboard.begin();
  pinMode(5, INPUT);
  pinMode(6, INPUT);
  pinMode(7, INPUT);
}

void loop() {
  if(digitalRead(5) == HIGH){
    if(prevState5 == LOW){
      Serial.println("press right button");
      Keyboard.print("Hello World!");
//      Keyboard.press(KEY_RETURN);
      delay(100);
      Keyboard.releaseAll();
    }

    else{
      Serial.println("holding right button");
    }

  }
  
  else if(digitalRead(6) == HIGH){
    if(prevState6 == LOW){
      Serial.println("press middle button");
      Keyboard.press(KEY_LEFT_GUI);
      Keyboard.press('a');
      delay(100);
      Keyboard.releaseAll();

      Keyboard.press(KEY_LEFT_GUI);
      Keyboard.press('c');
      delay(100);
      Keyboard.releaseAll();

      Keyboard.press(KEY_RIGHT_ARROW);
      delay(100);
      Keyboard.releaseAll();

      Keyboard.press(KEY_RETURN);
      delay(100);
      Keyboard.releaseAll();

      Keyboard.press(KEY_LEFT_GUI);
      Keyboard.press('v');
      delay(100);
      Keyboard.releaseAll();
      delay(100);
    }

    else{
      Serial.println("holding middle button");
    }
  }
  
  else if(digitalRead(7) == HIGH){
    if(prevState7 == LOW){
      Serial.println("press left button");
      Keyboard.press(KEY_LEFT_GUI);
      Keyboard.press('a');
      delay(100);
      Keyboard.releaseAll();

      Keyboard.press(KEY_LEFT_GUI);
      Keyboard.press(KEY_LEFT_SHIFT);
      Keyboard.press('l');
      delay(100);
      Keyboard.releaseAll();
      
    }
    else{
      Serial.println("holding left button");
    }
  }
  
  else{
  }
  
  prevState5 = digitalRead(5);
  prevState6 = digitalRead(6);
  prevState7 = digitalRead(7);
  
  delay(200);
  
}
