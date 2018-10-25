<template>
<div>
    <div id="signup-form" class="row no-gutters" >
        <div class="form rounded col-6 offset-3 p-4">
            <form novalidate v-on:submit.prevent>
                <div class="form-group ">
                    <label for="email">Adres e-mail</label>
                    <input
                            type="text"
                            id="email"
                            v-bind:class="[$v.email.$error || isEmailTaken ? invalidClass : '',
                                !$v.email.$invalid ? validClass : '', formClass]"
                            @keydown="isEmailTaken = false"
                            @blur="$v.email.$touch()"
                            v-model="email">
                    <p v-if="!$v.email.email">Proszę podać prawidłowy adres email.</p>
                    <p v-if="!$v.email.required">To pole nie może być puste.</p>
                    <p v-if="isEmailTaken">Ten adres email został już zajęty.</p>
                </div>
                <div class="form-group">
                    <label for="login">Login</label>
                    <input
                            type="text"
                            id="login"
                            v-bind:class="[$v.login.$error ? invalidClass : '',
                                !$v.login.$invalid ? validClass : '', formClass]"
                            @blur="$v.login.$touch()"
                            v-model.lazy="login">
                </div>
                <p v-if="!$v.login.unique">Ten login jest już zajęty!.</p>
                <p v-if="!$v.login.required">To pole nie może być puste.</p>
                <div class="submit">
                    <button
                        type="submit"
                        class="btn btn-info btn-block"
                        :disabled="$v.$invalid"
                        @click="onSubmit">Utwórz konto !</button>
                </div>
            </form>
        </div>
    </div>
</div>
</template>

<script>
import { required, email } from 'vuelidate/lib/validators'
import axios from 'axios'
export default {
  name: 'signup',
  data () {
    return {
      login: '',
      email: '',
      isEmailTaken: false,
      invalidClass: 'is-invalid',
      validClass: 'is-valid',
      formClass: 'form-control'
    }
  },
  methods: {
    onSubmit () {
      this.$store.dispatch('setSpinnerLoading')
      this.$store.dispatch('signUp', {
        email: this.email,
        login: this.login
      })
        .then(() => {
          this.$store.dispatch('unsetSpinnerLoading')
          this.$swal({
            title: 'Gratulacje !',
            text: 'Super, udało Ci się zakończyć proces tworzenia konta,' +
              'na podany adres e-mail została wiadmość z linkiem do resetowania hasła.',
            type: 'success'
          })
            .then(() => {
              this.$router.push('/')
            })
        })
        .catch(error => {
          this.$store.dispatch('unsetSpinnerLoading')
          if (error.response.status === 409) {
            this.isEmailTaken = true
          } else {
            this.$swal({
              title: 'Wystąpił nieoczekiwany błąd',
              text: 'Jeżeli nie wiesz co może być przyczyną błędu, proszę skontaktuj się z administratorem',
              type: 'error'
            })
          }
        })
    }
  },
  validations: {
    login: {
      required,
      unique: val => {
        if (val === '') return true
        return axios.head(`login?login=${val}`)
          .then(() => { return false })
          .catch(error => {
            return error.response.status === 404
          })
      }
    },
    email: {
      required,
      email
    }
  }
}
</script>

<style scoped>
</style>
