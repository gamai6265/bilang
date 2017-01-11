using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class HomeBtnsViewPresenter : ViewPresenter {
        public UILabel LblHome;
        public UILabel LblShopCar;
        public UILabel LblAppointment;
        public UILabel LblStory;

        private GameObject[] _btnList;
        public UIButton BtnAppointment;
        public UIButton BtnDesign;
        public UIButton BtnHome;
        public UIButton BtnShopCar;
        public UIButton BtnStory;

        public TweenPosition HomeBtns;

        public event MyAction ClickHome;
        public event MyAction ClickShopCar;
        public event MyAction ClickDesign;
        public event MyAction ClickAppointment;
        public event MyAction ClickStory;
        private Vector3 _from;
        private Vector3 _to;

        protected override void AwakeUnityMsg() {
            _btnList = new[] {
                BtnHome.gameObject, BtnShopCar.gameObject, null, BtnAppointment.gameObject,
                BtnStory.gameObject
            };
            BtnsListener();
            ChangeLanguage();
            _from = HomeBtns.from;
            _to = HomeBtns.to;
        }

        protected override void StartUnityMsg() {
        }

        private void ChangeLanguage() {
            LblHome.text = StrConfigProvider.Instance.GetStr("m_home");
            LblShopCar.text = StrConfigProvider.Instance.GetStr("m_shopcar");
            LblAppointment.text = StrConfigProvider.Instance.GetStr("m_appointment");
            LblStory.text = StrConfigProvider.Instance.GetStr("m_story");
            LblHome.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblShopCar.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblAppointment.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblStory.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        //按钮点击事件
        private void BtnsListener() {
            BtnHome.onClick.Add(new EventDelegate(OnBtnHomeClick));
            BtnDesign.onClick.Add(new EventDelegate(OnBtnDesignClick));
            BtnShopCar.onClick.Add(new EventDelegate(OnBtnShopCarClick));
            BtnAppointment.onClick.Add(new EventDelegate(OnBtnAppointmentClick));
            BtnStory.onClick.Add(new EventDelegate(OnBtnStoryClick));
        }

        //主页
        private void OnBtnHomeClick() {
            if (null != ClickHome) {
                ClickHome();
            }
        }

        //购物车
        private void OnBtnShopCarClick() {
            if (null != ClickShopCar) {
                ClickShopCar();
            }
        }

        //自主设计
        private void OnBtnDesignClick() {
            if (null != ClickDesign) {
                ClickDesign();
            }
        }

        //预约量身
        private void OnBtnAppointmentClick() {
            if (null != ClickAppointment) {
                ClickAppointment();
            }
        }

        //品牌故事
        private void OnBtnStoryClick() {
            if (null != ClickStory) {
                ClickStory();
            }
        }

        public void PlayAnimation(bool tof = false) {
            if (tof) {
                transform.localPosition = _to;
                TweenPosition.Begin(gameObject, HomeBtns.duration, _from);
            } else {
                transform.localPosition = _from;
                TweenPosition.Begin(gameObject, HomeBtns.duration, _to);
            }
        }

        public void ChangeBtnSprite(int index) {
            for (var k = 0; k < _btnList.Length; k++) {
                if (_btnList[k] != null) {
                    if (k == index) {
                        _btnList[k].GetComponentInChildren<UISprite>().alpha = 1f;
                        _btnList[k].GetComponentInChildren<UILabel>().color = Color.white;
                    } else {
                        _btnList[k].GetComponentInChildren<UISprite>().alpha = 0.5f;
                        _btnList[k].GetComponentInChildren<UILabel>().color = Color.gray;
                    }
                }
            }
        }
    }
}