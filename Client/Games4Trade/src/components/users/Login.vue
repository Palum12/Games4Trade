<template>
    <div id="login-form" class="row">
        <div class="form rounded col-4 offset-4 p-4">
            <form @submit.prevent="onSubmit">
                <div class="form-group">
                    <label for="login">Login</label>
                    <input
                            type="text"
                            id="login"
                            class="form-control"
                            v-model="user.login">
                </div>
                <div class="form-group">
                    <label for="password">Hasło</label>
                    <input
                            type="password"
                            id="password"
                            class="form-control"
                            v-model="user.password">
                </div>
                <div class="submit">
                    <button type="submit" class="btn btn-info">Zaloguj</button>
                </div>
            </form>
        </div>
    </div>
</template>

<script>
export default {
  name: 'Login',
  data () {
    return {
      user: {
        login: '',
        password: ''
      }
    }
  },
  methods: {
    onSubmit () {
      this.$store.dispatch('login', this.user)
    }
  }
}
</script>
<style scoped>
 .form{
     background-color: whitesmoke;
     border: solid 1px #26bba6;
 }
</style>
