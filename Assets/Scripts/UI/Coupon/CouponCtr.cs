using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class CouponCtr : IController {
        private CouponViewPresenter _view;
        private IOrderCenter _orderCenter;

        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<CouponViewPresenter>(WindowId.Coupon);
                _view.ClickBack += OnBackClick;
                _view.ClickChoose += OnClickChoose;
            }
            UiManager.Instance.ShowWindow(WindowId.Coupon);
            GetCouponData();
        }

        public void Update() {
        }

        void OnBackClick() {
            OnBack(-1);
        }

        public bool OnBack(int zorder) {
            Hide();
            return true;
        }

        private void GetCouponData() {
            _view.DeleteAll();
            UiControllerMgr.Instance.ShowLodingTips(true);
            var couponList = _orderCenter.CouponList["couponList"];
            for (var i = 0; i < couponList.Count; i++) {
                CoroutineMgr.Instance.StartCoroutine(IntCoupon(couponList[i]));
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        private IEnumerator IntCoupon(JsonData json) {
            var www = new WWW(Constants.CouponImageURL + Utils.GetJsonStr(json["suitsImg"]));
            yield return www;
            Texture2D texture = null;
            if (www.error == null) {
                if (www.isDone) {
                    texture = www.texture;
                }
            }
            else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                texture = www.texture;
            }
            www.Dispose();
            _view.AddPrefabs(json, texture);
        }

        private void OnClickChoose(string couponid, string couponPrice, string title) {
            EventCenter.Instance.Broadcast(EventId.ChooseCoupon, couponid, couponPrice, title);
            Hide();
        }

        public void Hide(MyAction onComplete = null) {
            UiManager.Instance.HideWindow(WindowId.Coupon);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Coupon, "ui/Coupon"),
            };
        }
    }
}