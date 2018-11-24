<template>
    <div id="login-form" class="row no-gutters">
        <div class="form rounded col-4 offset-4 p-4">
            <form @submit.prevent="changePassword">
                <div class="form-group">
                    <label for="password">Hasło</label>
                    <input
                            type="password"
                            id="password"
                            v-bind:class="[$v.password.$error ? invalidClass : '',
                                    !$v.password.$invalid ? validClass : '', formClass]"
                            @blur="$v.password.$touch()"
                            v-model="password">
                </div>
                <p v-if="!$v.password.minLen">Hasło musi mieć nie mniej niż {{ $v.password.$params.minLen.min }}
                    znaków!</p>
                <p v-if="!$v.password.required">To pole nie może być puste.</p>
                <div class="form-group">
                    <label for="confirm-password">Powtórz hasło</label>
                    <input
                            type="password"
                            id="confirm-password"
                            v-bind:class="[$v.confirmPassword.$error ? invalidClass : '',
                                !$v.confirmPassword.$invalid && $v.confirmPassword.$dirty ? validClass : '', formClass]"
                            @blur="$v.confirmPassword.$touch()"
                            v-model="confirmPassword">
                </div>
                <p v-if="!$v.confirmPassword.sameAs">Hasła muszą sie zgadzać !</p>
                <div class="submit">
                    <button type="submit"
                            class="btn btn-info btn-block"
                            @click="changePassword"
                            :disabled="$v.$invalid">Przywróć hasło</button>
                </div>
            </form>
        </div>
    </div>
</template>

<script>
import { required, sameAs, minLength } from 'vuelidate/lib/validators'
import axios from 'axios'
export default {
  name: 'ChangePassword',
  data () {
    return {
      recoveryString: '',
      password: '',
      confirmPassword: '',
      invalidClass: 'is-invalid',
      validClass: 'is-valid',
      formClass: 'form-control'
    }
  },
  methods: {
    changePassword () {
      this.$store.dispatch('setSpinnerLoading')
      axios.post('login/password/change', {
        recoveryString: this.recoveryString,
        newPassword: this.password
      })
        .then(() => {
          this.$store.dispatch('unsetSpinnerLoading')
          this.$swal({
            title: 'Hasło zostało zmienione',
            type: 'success'
          })
          this.$router.replace({name: 'home'})
        })
        .catch(() => {
          this.$store.dispatch('unsetSpinnerLoading')
          this.$swal({
            title: 'Wystąpił nieoczekiwany błąd',
            text: 'Jeżeli nie wiesz co może być przyczyną błędu, proszę skontaktuj się z administratorem',
            type: 'error'
          })
        })
    }
  },
  created () {
    this.recoveryString = this.$route.query.recoveryString
  },
  validations: {
    password: {
      required,
      minLen: minLength(6)
    },
    confirmPassword: {
      sameAs: sameAs('password')
    }
  }
}
</script>

<style scoped>
</style>
