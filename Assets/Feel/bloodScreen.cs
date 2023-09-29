using Cinemachine.PostFX;
using Shapes2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class bloodScreen : MonoBehaviour
{
    public GameObject player;
    public float originalLives;
    bool heartbeat = false;
    //hitEffect
    public Volume volume;
    private Bloom _Bloom;
    private Vignette _Vignette;
    private DepthOfField _DepthOfField;
    public Volume volume2;
    private Vignette _Vignette2;

    private bool blackcorneredScreen = false;
    private void Start()
    {
        originalLives = FindObjectOfType<PlayerBrainReader>().health;
        volume.profile.TryGet(out _Bloom);
        volume.profile.TryGet(out _Vignette);
        volume.profile.TryGet(out _DepthOfField);
        volume2.profile.TryGet(out _Vignette2);
    }
    private void Update()
    {
        if (player.GetComponent<Brain>()._Live == originalLives)
        {
            _Bloom.intensity.value = 0f;
            _Vignette.intensity.value = 0.3f;
            _DepthOfField.focalLength.value = 50f;
        }
        if (player.GetComponent<Brain>()._Live < originalLives)
        {
            //death
            //stop time
            //fade last screen
        }
        if (player.GetComponent<Brain>()._Live < originalLives / 2f)
        {
            //1rst screen
            FindObjectOfType<CameraShake>().ShakeCamera(0.1f, 0.1f);
            _Vignette.intensity.value = 0.34f;
            _Bloom.intensity.value = 0f;
            _DepthOfField.focalLength.value = 80f;
        }

        if (player.GetComponent<Brain>()._Live < originalLives / 3f)
        {
            //2nd screen
            FindObjectOfType<CameraShake>().ShakeCamera(0.2f, 0.2f);
            _Vignette.intensity.value = 0.37f;
            _Bloom.intensity.value = 1f;
            _DepthOfField.focalLength.value = 110f;

        }
        else if(player.GetComponent<Brain>()._Live < originalLives / 2f)
        {
            _Vignette.intensity.value = 0.34f;
            _Bloom.intensity.value = 0f;
            _DepthOfField.focalLength.value = 80f;
        }
        if (player.GetComponent<Brain>()._Live < originalLives/4f)
        {
            //3rd screen
            //intense heartbeat
            FindObjectOfType<CameraShake>().ShakeCamera(0.3f, 0.2f);
            _Bloom.intensity.value = 1.5f;
            _Vignette.intensity.value = 0.41f;
            _DepthOfField.focalLength.value = 150f;
            if (heartbeat == false)
            {
                heartbeat = true;
                FindObjectOfType<AudioManager>().Play("Heart");
            }

        }
        else if (player.GetComponent<Brain>()._Live < originalLives / 3f)
        {
            _Vignette.intensity.value = 0.37f;
            _Bloom.intensity.value = 1f;
            _DepthOfField.focalLength.value = 110f;
            if (heartbeat == true)
            {
                heartbeat = false;
                FindObjectOfType<AudioManager>().Stop("Heart");
            }
        }
        if (blackcorneredScreen)
        {
            if (_Vignette2.intensity.value < 0.5f)
            {
                if (_Vignette2.intensity.value > _Vignette.intensity.value) volume2.priority = 1;
                _Vignette2.intensity.value += 2f * Time.deltaTime;
            }
            if (_Vignette2.intensity.value > 0.5f)
            {
                _Vignette2.intensity.value = 0.5f;
                blackcorneredScreen = false;
            }
            if (_Vignette2.intensity.value == 0.5f) blackcorneredScreen = false;
        }
        else
        {
            if (_Vignette2.intensity.value > 0.3f)
            {
                if (_Vignette2.intensity.value < _Vignette.intensity.value) volume2.priority = -1;
                _Vignette2.intensity.value -= 2f * Time.deltaTime;
            }
            if (_Vignette2.intensity.value < 0.3f)
            {
                _Vignette2.intensity.value = 0.3f; 
                volume2.priority = -1;
            }
        }

    }
    public void Hit()
    {
        StartCoroutine(HIT());
    }
    public IEnumerator HIT()
    {
        //play a sound?
        //cameraShake
        FindObjectOfType<CameraShake>().ShakeCamera(2f, 0.15f);
        //pausebreak
        FindObjectOfType<HitPause>().PauseHit(0.15f);
        //black cornered screen
        blackcorneredScreen = true;
        _Vignette2.intensity.value = 0.3f;

        //flashPlayerEffect
        Color PlayerColor = FindObjectOfType<Followplayer>().gameObject.GetComponent<SpriteRenderer>().color;
        FindObjectOfType<Followplayer>().gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.1f);
        FindObjectOfType<Followplayer>().gameObject.GetComponent<SpriteRenderer>().color = PlayerColor;



    }
}
