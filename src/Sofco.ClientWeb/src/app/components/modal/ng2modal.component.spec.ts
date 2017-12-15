import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { Ng2ModalComponent } from './ng2modal.component';

describe('Ng2ModalComponent', () => {
  let component: Ng2ModalComponent;
  let fixture: ComponentFixture<Ng2ModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Ng2ModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Ng2ModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
