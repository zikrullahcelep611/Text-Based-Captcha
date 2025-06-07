import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCaptchaComponent } from './create-captcha.component';

describe('CreateCaptchaComponent', () => {
  let component: CreateCaptchaComponent;
  let fixture: ComponentFixture<CreateCaptchaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateCaptchaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCaptchaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
