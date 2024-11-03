using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UpgradeType
{
    MaxHealth,
    BulletDamage,
    BulletForce,
    PushDamage,
    PushForce,
    Speed,
    HealthRegen,
    BulletWidth
}

public class UpgradeShop : MonoBehaviour
{
    public UpgradeType upgradeType;
    public int cost;
    // E.g. 0.25 to increase by 25%
    public float multiplier;

    public GameObject costText;
    public GameObject descText;
    
    private TextMeshProUGUI _costText;
    private TextMeshProUGUI _descText;
    private PlayerController _playerController;
    
    private static readonly Dictionary<UpgradeType, string> EnumToTitle = new()
    {
        {UpgradeType.MaxHealth, "Max Health"},
        {UpgradeType.BulletDamage, "Bullet Damage"},
        {UpgradeType.BulletForce, "Bullet Force"},
        {UpgradeType.PushDamage, "Push Damage"},
        {UpgradeType.PushForce, "Push Force"},
        {UpgradeType.Speed, "Speed"},
        {UpgradeType.HealthRegen, "Health Regen"},
        {UpgradeType.BulletWidth, "Bullet Width"}
    };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _descText = descText.GetComponent<TextMeshProUGUI>();
        _costText = costText.GetComponent<TextMeshProUGUI>();
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _descText.text = EnumToTitle[upgradeType] + " +" + String.Format("{0:P0}", multiplier);
        _costText.text = "Cost: " + cost;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerController.money >= cost)
        {
            _costText.color = Color.green;
        }
        else
        {
            _costText.color = Color.red;
        }
    }
    
    public void Buy ()
    {
        if (_playerController.money >= cost)
        {
            _playerController.SubtractMoney(cost);
            _playerController.Upgrades[upgradeType]++;
            switch (upgradeType)
            {
                case UpgradeType.MaxHealth:
                    _playerController.MaxHealth = (int)(_playerController.maxHealthBase *
                                                        (1 + multiplier *
                                                            _playerController.Upgrades[UpgradeType.MaxHealth]));
                    break;
                case UpgradeType.BulletDamage:
                    _playerController.BulletDamage = (int)(_playerController.bulletDamageBase *
                                                           (1 + multiplier *
                                                               _playerController.Upgrades[UpgradeType.BulletDamage]));
                    break;
                case UpgradeType.BulletForce:
                    _playerController.BulletForce = (int)(_playerController.bulletForceBase *
                                                          (1 + multiplier *
                                                              _playerController.Upgrades[UpgradeType.BulletForce]));
                    break;
                case UpgradeType.PushDamage:
                    _playerController.PushDamage = (int)(_playerController.pushDamageBase *
                                                         (1 + multiplier *
                                                             _playerController.Upgrades[UpgradeType.PushDamage]));
                    break;
                case UpgradeType.PushForce:
                    _playerController.PushForce = (int)(_playerController.pushForceBase *
                                                        (1 + multiplier *
                                                            _playerController.Upgrades[UpgradeType.PushForce]));
                    break;
                case UpgradeType.Speed:
                    _playerController.Speed = (int)(_playerController.speedBase *
                                                    (1 + multiplier * _playerController.Upgrades[UpgradeType.Speed]));
                    break;
                case UpgradeType.HealthRegen:
                    _playerController.HealthRegen = (_playerController.healthRegenBase *
                                                          (1 + multiplier *
                                                              _playerController.Upgrades[UpgradeType.HealthRegen]));
                    break;
                case UpgradeType.BulletWidth:
                    _playerController.BulletWidth = (_playerController.bulletWidthBase *
                                                          (1 + multiplier *
                                                              _playerController.Upgrades[UpgradeType.BulletWidth]));
                    break;
            }
        }
    }
}
