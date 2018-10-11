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
                            class="form-control"
                            @blur="$v.login.$touch()"
                            v-model.lazy="login">
                </div>
                <p v-if="!$v.login.unique">Ten login jest już zajęty!.</p>
                <p v-if="!$v.login.required">To pole nie może być puste.</p>
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
                <p v-if="!$v.password.minLen">Hasło musi mieć nie mniej niż {{ $v.password.$params.minLen.min }} znaków!</p>
                <p v-if="!$v.password.required">To pole nie może być puste.</p>
                <div class="form-group">
                    <label for="confirmPassword">Powtórz hasło</label>
                    <input
                            type="password"
                            id="confirmPassword"
                            v-bind:class="[$v.confirmPassword.$error ? invalidClass : '',
                                !$v.confirmPassword.$invalid && $v.confirmPassword.$dirty ? validClass : '', formClass]"
                            @blur="$v.confirmPassword.$touch()"
                            v-model="confirmPassword">
                </div>
                <p v-if="!$v.confirmPassword.sameAs">Hasła muszą sie zgadzać !</p>
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
import { required, email, sameAs, minLength } from 'vuelidate/lib/validators'
import axios from 'axios'
export default {
  name: 'signup',
  data () {
    return {
      login: '',
      password: '',
      confirmPassword: '',
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
        login: this.login,
        password: this.password
      })
        .then(() => {
          this.$store.dispatch('unsetSpinnerLoading')
          this.$swal({
            title: 'Gratulacje !',
            text: 'Super, udało Ci się zakończyć proces tworzenia konta,' +
              'teraz tylko się na nie zaloguj i korzystaj z naszego serwisu.',
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
    },
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
